using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Models.Requests;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {
        private static readonly HashSet<string> AllowedImageTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        private const long MaxImageSizeBytes = 2 * 1024 * 1024;
        private static readonly Regex CuisineRegex = new(@"^[\p{L}\s\-]+$", RegexOptions.Compiled);

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public RecipeController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe([FromForm] CreateRecipeRequest request)
        {
            var validationErrors = ValidateCreateRecipeRequest(request, out var ingredients, out var steps);
            if (validationErrors.Count > 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = validationErrors[0],
                    errors = validationErrors
                });
            }

            await SyncPrimaryKeySequencesAsync();

            int categoryId;
            Category? cuisineCategory = null;
            if (request.CuisineId.HasValue)
            {
                cuisineCategory = await _context.Categories.FindAsync(request.CuisineId.Value);
                categoryId = cuisineCategory != null ? cuisineCategory.Id : ResolveCategoryId(request.Cuisine);
            }
            else
            {
                categoryId = ResolveCategoryId(request.Cuisine);
                cuisineCategory = await _context.Categories.FindAsync(categoryId);
            }

            var slug = await GenerateUniqueSlugAsync(request.Title);
            var imageFolder = Path.Combine(_environment.WebRootPath, "images", "user");
            Directory.CreateDirectory(imageFolder);

            var mainImageFileName = request.MainImage != null
                ? await SaveImageAsync(request.MainImage, imageFolder, slug + "-main")
                : "salads.png";

            var recipe = new Recipe
            {
                Name = request.Title.Trim(),
                Author = string.IsNullOrWhiteSpace(request.Author) ? null : request.Author.Trim(),
                Slug = slug,
                Description = request.Description.Trim(),
                Cuisine = cuisineCategory?.DisplayName ?? (string.IsNullOrWhiteSpace(request.Cuisine) ? null : request.Cuisine.Trim()),
                Difficulty = request.Difficulty,
                ImageFileName = mainImageFileName,
                IsFavorite = false,
                CookingTime = request.CookingTime > 0 ? request.CookingTime : 30,
                CategoryId = categoryId,
                CreatedAt = DateTime.UtcNow,
                RatingSum = 0,
                RatingCount = 0,
                Ingredients = ingredients
                    .Select((text, idx) => new RecipeIngredient
                    {
                        DisplayText = text,
                        SortOrder = idx + 1
                    })
                    .ToList(),
                Steps = new List<RecipeStep>()
            };

            for (var i = 0; i < steps.Count; i++)
            {
                var stepImagePath = "salads.png";
                if (i < request.StepImages.Count && request.StepImages[i] != null && request.StepImages[i].Length > 0)
                {
                    stepImagePath = await SaveImageAsync(request.StepImages[i], imageFolder, $"{slug}-step-{i + 1}");
                }

                recipe.Steps.Add(new RecipeStep
                {
                    StepNumber = i + 1,
                    Description = steps[i],
                    ImagePath = stepImagePath
                });
            }

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Рецепт успешно сохранен",
                data = new
                {
                    recipe.Id,
                    recipe.Slug
                }
            });
        }

        /// <summary>
        /// Get all recipes with optional search, filters and pagination
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string query = "",
            [FromQuery] string[]? categories = null,
            [FromQuery] int[]? difficulties = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 4)
        {
            try
            {
                var recipesQuery = _context.Recipes.Include(r => r.Category).AsQueryable();

                // Search filter
                if (!string.IsNullOrWhiteSpace(query))
                {
                    query = query.ToLower().Trim();
                    recipesQuery = recipesQuery.Where(r =>
                        (r.Name ?? string.Empty).ToLower().Contains(query) ||
                        (r.Description ?? string.Empty).ToLower().Contains(query));
                }

                // Category filter
                if (categories != null && categories.Length > 0)
                {
                    recipesQuery = recipesQuery.Where(r =>
                        r.Category != null &&
                        r.Category.Name != null &&
                        categories.Contains(r.Category.Name));
                }

                // Difficulty filter
                if (difficulties != null && difficulties.Length > 0)
                {
                    recipesQuery = recipesQuery.Where(r =>
                        difficulties.Contains(r.Difficulty));
                }

                var totalCount = await recipesQuery.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var recipes = await recipesQuery
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => new
                    {
                        r.Id,
                        r.Name,
                        r.Slug,
                        r.Description,
                        r.Difficulty,
                        r.ImageFileName,
                        r.CookingTime,
                        Category = r.Category != null ? r.Category.DisplayName : null,
                        r.RatingSum,
                        r.RatingCount,
                        AverageRating = r.RatingCount > 0 ? (double)r.RatingSum / r.RatingCount : 0.0
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = recipes,
                    count = totalCount,
                    page = page,
                    pageSize = pageSize,
                    totalPages = totalPages
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Rate a recipe (1-5 stars)
        /// </summary>
        [HttpPost("{id}/rate")]
        public async Task<IActionResult> RateRecipe(int id, [FromBody] RateRequest request)
        {
            try
            {
                if (request.Rating < 1 || request.Rating > 5)
                    return BadRequest(new { success = false, message = "Оценка должна быть от 1 до 5" });

                var recipe = await _context.Recipes.FindAsync(id);
                if (recipe == null)
                    return NotFound(new { success = false, message = "Рецепт не найден" });

                recipe.RatingSum += request.Rating;
                recipe.RatingCount++;
                await _context.SaveChangesAsync();

                double averageRating = (double)recipe.RatingSum / recipe.RatingCount;
                return Ok(new
                {
                    success = true,
                    averageRating = Math.Round(averageRating, 1),
                    ratingCount = recipe.RatingCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get search suggestions (autocomplete)
        /// </summary>
        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions([FromQuery] string query = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query) || query.Length < 1)
                {
                    return Ok(new { success = true, data = new List<object>() });
                }

                query = query.ToLower().Trim();

                var suggestions = await _context.Recipes
                    .Where(r =>
                        (r.Name ?? string.Empty).ToLower().Contains(query) ||
                        (r.Description ?? string.Empty).ToLower().Contains(query))
                    .OrderBy(r => r.Name)
                    .Take(10)
                    .Select(r => new
                    {
                        r.Id,
                        r.Name,
                        type = "recipe"
                    })
                    .ToListAsync();

                // Also add category suggestions
                var categorySuggestions = await _context.Categories
                    .Where(c => (c.Name ?? string.Empty).ToLower().Contains(query) ||
                               (c.DisplayName ?? string.Empty).ToLower().Contains(query))
                    .OrderBy(c => c.DisplayName)
                    .Take(5)
                    .Select(c => new
                    {
                        c.Id,
                        Name = c.DisplayName,
                        type = "category"
                    })
                    .ToListAsync();

                var allSuggestions = suggestions.Cast<object>().Concat(categorySuggestions.Cast<object>()).ToList();

                return Ok(new { success = true, data = allSuggestions });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all cuisines (categories)
        /// </summary>
        [HttpGet("cuisines")]
        public async Task<IActionResult> GetCuisines()
        {
            try
            {
                var cuisines = await _context.Categories
                    .Select(c => new { c.Id, c.Name, c.DisplayName })
                    .OrderBy(c => c.DisplayName)
                    .ToListAsync();
                return Ok(new { success = true, cuisines });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Create a new cuisine (category)
        /// </summary>
        [HttpPost("cuisines")]
        public async Task<IActionResult> CreateCuisine([FromBody] CreateCuisineRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.DisplayName))
                return BadRequest(new { success = false, message = "Укажите название кухни" });

            var displayName = request.DisplayName.Trim();
            if (displayName.Length < 2 || displayName.Length > 60)
                return BadRequest(new { success = false, message = "Название кухни должно быть от 2 до 60 символов" });

            if (!CuisineRegex.IsMatch(displayName))
                return BadRequest(new { success = false, message = "Название кухни может содержать только буквы, пробелы и дефисы" });

            var existing = await _context.Categories
                .FirstOrDefaultAsync(c => c.DisplayName.ToLower() == displayName.ToLower());
            if (existing != null)
                return Ok(new { success = true, id = existing.Id, name = existing.Name, displayName = existing.DisplayName, isNew = false });

            await _context.Database.ExecuteSqlRawAsync(@"
SELECT setval(pg_get_serial_sequence('""Categories""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""Categories"";
");

            var name = ToSlug(displayName);
            if (string.IsNullOrWhiteSpace(name)) name = "cuisine";

            var category = new Category { Name = name, DisplayName = displayName };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, id = category.Id, name = category.Name, displayName = category.DisplayName, isNew = true });
        }

        /// <summary>
        /// Get all available categories and difficulties for filter UI
        /// </summary>
        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters()
        {
            try
            {
                var categories = await _context.Categories
                    .Select(c => new { c.Id, c.Name, c.DisplayName })
                    .OrderBy(c => c.DisplayName)
                    .ToListAsync();

                var difficulties = new[]
                {
                    new { Id = 1, Name = "Easy", DisplayName = "Лёгкое" },
                    new { Id = 2, Name = "Medium", DisplayName = "Среднее" },
                    new { Id = 3, Name = "Hard", DisplayName = "Сложное" }
                };

                return Ok(new
                {
                    success = true,
                    categories = categories,
                    difficulties = difficulties
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private async Task<string> GenerateUniqueSlugAsync(string title)
        {
            var baseSlug = ToSlug(title);
            if (string.IsNullOrWhiteSpace(baseSlug))
            {
                baseSlug = "recipe";
            }

            var slug = baseSlug;
            var index = 1;
            while (await _context.Recipes.AnyAsync(r => r.Slug == slug))
            {
                slug = $"{baseSlug}-{index}";
                index++;
            }

            return slug;
        }

        private static string ToSlug(string text)
        {
            var normalized = text.Trim().ToLowerInvariant();
            var chars = normalized
                .Select(ch => char.IsLetterOrDigit(ch) ? ch : '-')
                .ToArray();
            var slug = new string(chars);
            while (slug.Contains("--"))
            {
                slug = slug.Replace("--", "-");
            }

            return slug.Trim('-');
        }

        private static int ResolveCategoryId(string? cuisine)
        {
            if (string.IsNullOrWhiteSpace(cuisine))
            {
                return 2;
            }

            var c = cuisine.ToLowerInvariant();
            if (c.Contains("рус") || c.Contains("slav") || c.Contains("борщ"))
            {
                return 1;
            }

            if (c.Contains("ази") || c.Contains("asia") || c.Contains("thai") || c.Contains("китай") || c.Contains("япон"))
            {
                return 3;
            }

            return 2;
        }

        private static async Task<string> SaveImageAsync(IFormFile file, string folderPath, string filePrefix)
        {
            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".jpg";
            }

            var safeExt = extension.ToLowerInvariant();
            if (safeExt != ".jpg" && safeExt != ".jpeg" && safeExt != ".png" && safeExt != ".webp")
            {
                safeExt = ".jpg";
            }

            var fileName = $"{filePrefix}-{Guid.NewGuid():N}{safeExt}";
            var fullPath = Path.Combine(folderPath, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"user/{fileName}";
        }

        private List<string> ValidateCreateRecipeRequest(CreateRecipeRequest request, out List<string> normalizedIngredients, out List<string> normalizedSteps)
        {
            var errors = new List<string>();

            var title = request.Title?.Trim() ?? string.Empty;
            var description = request.Description?.Trim() ?? string.Empty;
            var author = request.Author?.Trim() ?? string.Empty;
            var cuisine = request.Cuisine?.Trim() ?? string.Empty;

            if (title.Length < 3 || title.Length > 120)
            {
                errors.Add("Название должно быть от 3 до 120 символов");
            }

            if (description.Length < 10 || description.Length > 250)
            {
                errors.Add("Описание должно быть от 10 до 250 символов");
            }

            if (!string.IsNullOrEmpty(author) && (author.Length < 2 || author.Length > 60))
            {
                errors.Add("Имя автора должно быть от 2 до 60 символов или пустым");
            }

            if (!string.IsNullOrEmpty(cuisine))
            {
                if (cuisine.Length < 3 || cuisine.Length > 60)
                {
                    errors.Add("Поле \"Кухня\" должно быть от 3 до 60 символов");
                }
                else if (!CuisineRegex.IsMatch(cuisine))
                {
                    errors.Add("Поле \"Кухня\" может содержать только буквы, пробелы и дефисы");
                }
            }

            if (request.Difficulty < 1 || request.Difficulty > 3)
            {
                errors.Add("Неверная сложность");
            }

            if (request.CookingTime <= 0 || request.CookingTime > 1440)
            {
                errors.Add("Время приготовления должно быть от 1 до 1440 минут");
            }

            normalizedIngredients = request.Ingredients
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Select(i => i.Trim())
                .ToList();

            if (normalizedIngredients.Count == 0)
            {
                errors.Add("Добавьте хотя бы один ингредиент");
            }

            for (var i = 0; i < normalizedIngredients.Count; i++)
            {
                var ingredient = normalizedIngredients[i];
                if (ingredient.Length > 140)
                {
                    errors.Add($"Ингредиент №{i + 1} слишком длинный");
                    continue;
                }

                var separatorIndex = ingredient.IndexOf('—');
                if (separatorIndex <= 0 || separatorIndex >= ingredient.Length - 1)
                {
                    errors.Add($"Ингредиент №{i + 1} должен содержать название и количество");
                    continue;
                }

                var ingredientName = ingredient[..separatorIndex].Trim();
                var ingredientAmount = ingredient[(separatorIndex + 1)..].Trim();
                if (ingredientName.Length == 0 || ingredientAmount.Length == 0)
                {
                    errors.Add($"Ингредиент №{i + 1} должен содержать название и количество");
                }
            }

            normalizedSteps = request.Steps
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();

            if (normalizedSteps.Count == 0)
            {
                errors.Add("Добавьте хотя бы один шаг");
            }

            for (var i = 0; i < normalizedSteps.Count; i++)
            {
                var step = normalizedSteps[i];
                if (step.Length < 10 || step.Length > 1000)
                {
                    errors.Add($"Шаг №{i + 1} должен быть от 10 до 1000 символов");
                }
            }

            if (request.StepImages.Count > normalizedSteps.Count)
            {
                errors.Add("Количество изображений шагов не должно превышать количество шагов");
            }

            if (request.MainImage != null && request.MainImage.Length > 0)
            {
                var mainImageError = ValidateImageFile(request.MainImage);
                if (mainImageError != null)
                {
                    errors.Add($"Основное изображение: {mainImageError}");
                }
            }

            for (var i = 0; i < request.StepImages.Count; i++)
            {
                var stepImage = request.StepImages[i];
                if (stepImage == null || stepImage.Length == 0)
                {
                    continue;
                }

                var stepImageError = ValidateImageFile(stepImage);
                if (stepImageError != null)
                {
                    errors.Add($"Изображение шага №{i + 1}: {stepImageError}");
                }
            }

            return errors;
        }

        private static string? ValidateImageFile(IFormFile file)
        {
            if (!AllowedImageTypes.Contains(file.ContentType))
            {
                return "допустимы только JPG, PNG или WebP";
            }

            if (file.Length > MaxImageSizeBytes)
            {
                return "размер не должен превышать 2 МБ";
            }

            return null;
        }

        private async Task SyncPrimaryKeySequencesAsync()
        {
            await _context.Database.ExecuteSqlRawAsync(@"
SELECT setval(pg_get_serial_sequence('""Recipes""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""Recipes"";
SELECT setval(pg_get_serial_sequence('""RecipeIngredients""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""RecipeIngredients"";
SELECT setval(pg_get_serial_sequence('""RecipeSteps""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""RecipeSteps"";
");
        }
    }

}
