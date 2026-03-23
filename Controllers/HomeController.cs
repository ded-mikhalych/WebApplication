using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
<<<<<<< HEAD
        _logger = logger;
        _context = context;
    }

    public Task<IActionResult> Index()
    {
        return Task.FromResult<IActionResult>(RedirectToPage("/Index"));
    }

    public IActionResult Privacy()
    {
        return RedirectToPage("/About");
    }

    public IActionResult Catalog()
    {
        return RedirectToPage("/Catalog");
    }

    public Task<IActionResult> Favorites()
    {
        return Task.FromResult<IActionResult>(RedirectToPage("/Favorites"));
    }

    public IActionResult AddRecipe()
    {
        return RedirectToPage("/AddRecipe");
    }

    public IActionResult About()
    {
        return RedirectToPage("/About");
    }

    public IActionResult InDevelopment()
    {
        return RedirectToPage("/InDevelopment");
    }

    public Task<IActionResult> Article(int id)
    {
        return Task.FromResult<IActionResult>(RedirectToPage("/Article", new { id }));
    }

    // Backward-compatibility redirect
    public async Task<IActionResult> Article3()
    {
        var article = await _context.News
            .FirstOrDefaultAsync(n => n.Title == "РџРѕРґСЃР»Р°СЃС‚РёС‚РµР»Рё РїРѕРґ СѓРіСЂРѕР·РѕР№");

        if (article == null)
        {
            return RedirectToPage("/InDevelopment");
=======
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
>>>>>>> 10dc85df8abcebded754389f0b2c5261312ceb18
        }

        return RedirectToPage("/Article", new { id = article.Id });
    }

    public Task<IActionResult> Soups()
    {
        return Task.FromResult<IActionResult>(RedirectToPage("/Soups"));
    }

    [HttpGet("home/recipe/{slug}")]
    public Task<IActionResult> Recipe(string slug)
    {
        return Task.FromResult<IActionResult>(RedirectToPage("/Recipe", new { slug }));
    }

    public IActionResult Kharcho()
    {
        return RedirectToPage("/Recipe", new { slug = "kharcho" });
    }

    public IActionResult Mushrooms()
    {
        return RedirectToPage("/Recipe", new { slug = "mushrooms" });
    }

<<<<<<< HEAD
    public IActionResult Olivie()
    {
        return RedirectToPage("/Recipe", new { slug = "olivie" });
    }

    public IActionResult Borsch()
    {
        return RedirectToPage("/Recipe", new { slug = "borsch" });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return RedirectToPage("/ErrorPage");
=======
        public IActionResult Catalog()
        {
            return View();
        }

        public async Task<IActionResult> Favorites()
        {
            var favoriteRecipes = await _context.Recipes
                .Include(r => r.Category)
                .Where(r => r.IsFavorite)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var model = favoriteRecipes.Select(r => new RecipeLinkViewModel
            {
                Name = r.Name ?? string.Empty,
                ActionName = string.IsNullOrWhiteSpace(r.Slug) ? nameof(InDevelopment) : nameof(Recipe),
                Slug = r.Slug
            }).ToList();

            return View(model);
        }

        public IActionResult AddRecipe()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult InDevelopment()
        {
            return View();
        }

        public async Task<IActionResult> Article(int id)
        {
            var article = await _context.News.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            var model = new ArticleViewModel
            {
                Title = article.Title ?? string.Empty,
                ImageSrc = ResolveImagePath(article.ImageFileName),
                Summary = article.Summary ?? string.Empty,
                ContentHtml = article.ContentHtml
            };

            return View(model);
        }

        public async Task<IActionResult> Soups()
        {
            var soups = await _context.Recipes
                .Include(r => r.Category)
                .Where(r => r.Category != null && r.Category.Name == "Russian")
                .OrderBy(r => r.Name)
                .ToListAsync();

            var model = soups.Select(r => new RecipeLinkViewModel
            {
                Name = r.Name ?? string.Empty,
                ActionName = string.IsNullOrWhiteSpace(r.Slug) ? nameof(InDevelopment) : nameof(Recipe),
                Slug = r.Slug
            }).ToList();

            return View(model);
        }

        [HttpGet("home/recipe/{slug}")]
        public async Task<IActionResult> Recipe(string slug)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .FirstOrDefaultAsync(r => r.Slug == slug);

            if (recipe == null)
            {
                return RedirectToAction(nameof(InDevelopment));
            }

            var model = new RecipePageViewModel
            {
                Name = recipe.Name ?? string.Empty,
                AuthorText = string.IsNullOrWhiteSpace(recipe.Author) ? "Не указан" : recipe.Author,
                Description = recipe.Description ?? string.Empty,
                MainImageSrc = ResolveImagePath(recipe.ImageFileName),
                DifficultyText = GetDifficultyText(recipe.Difficulty),
                CuisineText = string.IsNullOrWhiteSpace(recipe.Cuisine)
                    ? recipe.Category?.DisplayName ?? "Не указано"
                    : recipe.Cuisine,
                RatingText = recipe.RatingCount > 0
                    ? ((double)recipe.RatingSum / recipe.RatingCount).ToString("0.0")
                    : "—",
                Ingredients = recipe.Ingredients
                    .OrderBy(i => i.SortOrder)
                    .Select(i => i.DisplayText)
                    .ToList(),
                Steps = recipe.Steps
                    .OrderBy(s => s.StepNumber)
                    .Select(s => new RecipeStepViewModel
                    {
                        StepNumber = s.StepNumber,
                        Description = s.Description,
                        ImageSrc = ResolveImagePath(s.ImagePath)
                    })
                    .ToList()
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static string ResolveImagePath(string? imageFileName)
        {
            if (string.IsNullOrWhiteSpace(imageFileName))
            {
                return "/images/placeholder.png";
            }

            return imageFileName.StartsWith("https://")
                ? imageFileName
                : $"/images/{imageFileName}";
        }

        private static string GetDifficultyText(int difficulty)
        {
            return difficulty switch
            {
                1 => "Легко",
                2 => "Средне",
                3 => "Сложно",
                _ => "Не указано"
            };
        }
>>>>>>> 10dc85df8abcebded754389f0b2c5261312ceb18
    }
}
