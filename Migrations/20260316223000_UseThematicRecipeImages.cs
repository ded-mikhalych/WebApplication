using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WebApplication.Data;

#nullable disable

namespace WebApplication.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260316223000_UseThematicRecipeImages")]
    public partial class UseThematicRecipeImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE ""Recipes""
SET ""ImageFileName"" = CASE ""Id""
    WHEN 7 THEN 'https://upload.wikimedia.org/wikipedia/commons/0/0a/Traditional_Uzbek_Pilaf_in_a_Cauldron_on_a_Potbelly_Stove_Moscow_Region.jpg'
    WHEN 8 THEN 'https://upload.wikimedia.org/wikipedia/commons/9/9c/Tom_Yum_Soup.JPG'
    WHEN 9 THEN 'https://upload.wikimedia.org/wikipedia/commons/a/ab/Ratatouille%2C_Mazatl%C3%A1n%2C_21_de_junio_de_2023.jpg'
    WHEN 10 THEN 'https://upload.wikimedia.org/wikipedia/commons/0/0d/Beef_Stroganoff-01.jpg'
    ELSE ""ImageFileName""
END
WHERE ""Id"" BETWEEN 7 AND 10;
");

            migrationBuilder.Sql(@"
UPDATE ""RecipeSteps""
SET ""ImagePath"" = CASE
    WHEN ""Id"" BETWEEN 40 AND 47 THEN 'https://upload.wikimedia.org/wikipedia/commons/0/0a/Traditional_Uzbek_Pilaf_in_a_Cauldron_on_a_Potbelly_Stove_Moscow_Region.jpg'
    WHEN ""Id"" BETWEEN 48 AND 55 THEN 'https://upload.wikimedia.org/wikipedia/commons/9/9c/Tom_Yum_Soup.JPG'
    WHEN ""Id"" BETWEEN 56 AND 60 THEN 'https://upload.wikimedia.org/wikipedia/commons/a/ab/Ratatouille%2C_Mazatl%C3%A1n%2C_21_de_junio_de_2023.jpg'
    WHEN ""Id"" BETWEEN 61 AND 67 THEN 'https://upload.wikimedia.org/wikipedia/commons/0/0d/Beef_Stroganoff-01.jpg'
    ELSE ""ImagePath""
END
WHERE ""Id"" BETWEEN 40 AND 67;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE ""Recipes""
SET ""ImageFileName"" = CASE ""Id""
    WHEN 7 THEN 'https://picsum.photos/seed/plov-main/1200/800'
    WHEN 8 THEN 'https://picsum.photos/seed/tom-yum-main/1200/800'
    WHEN 9 THEN 'https://picsum.photos/seed/ratatouille-main/1200/800'
    WHEN 10 THEN 'https://picsum.photos/seed/beef-stroganoff-main/1200/800'
    ELSE ""ImageFileName""
END
WHERE ""Id"" BETWEEN 7 AND 10;
");

            migrationBuilder.Sql(@"
UPDATE ""RecipeSteps""
SET ""ImagePath"" = CASE
    WHEN ""Id"" BETWEEN 40 AND 47 THEN 'https://picsum.photos/seed/plov-step/1000/700'
    WHEN ""Id"" BETWEEN 48 AND 55 THEN 'https://picsum.photos/seed/tomyam-step/1000/700'
    WHEN ""Id"" BETWEEN 56 AND 60 THEN 'https://picsum.photos/seed/ratatouille-step/1000/700'
    WHEN ""Id"" BETWEEN 61 AND 67 THEN 'https://picsum.photos/seed/stroganoff-step/1000/700'
    ELSE ""ImagePath""
END
WHERE ""Id"" BETWEEN 40 AND 67;
");
        }
    }
}
