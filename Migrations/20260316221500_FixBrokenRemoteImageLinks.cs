using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WebApplication.Data;

#nullable disable

namespace WebApplication.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260316221500_FixBrokenRemoteImageLinks")]
    public partial class FixBrokenRemoteImageLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE ""Recipes""
SET ""ImageFileName"" = CASE ""Id""
    WHEN 7 THEN 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=600'
    WHEN 8 THEN 'https://images.unsplash.com/photo-1589301277067-5c516e11eef0?w=600'
    WHEN 9 THEN 'https://images.unsplash.com/photo-1592521473069-d8b71fa5c9a0?w=600'
    WHEN 10 THEN 'https://images.unsplash.com/photo-1642019492077-999e179c188c?w=600'
    ELSE ""ImageFileName""
END
WHERE ""Id"" BETWEEN 7 AND 10;
");

            migrationBuilder.Sql(@"
UPDATE ""RecipeSteps""
SET ""ImagePath"" = CASE
    WHEN ""Id"" IN (40,41,48,56,61) THEN 'https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=600'
    WHEN ""Id"" IN (42,43,57,58,59,62,63,64,65) THEN 'https://images.unsplash.com/photo-1495521821757-a1efb6729352?w=600'
    WHEN ""Id"" IN (44,45,46,49,50,51,52,53,66) THEN 'https://images.unsplash.com/photo-1565958011457-541ef9c9ac50?w=600'
    WHEN ""Id"" IN (47) THEN 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=600'
    WHEN ""Id"" IN (54,55) THEN 'https://images.unsplash.com/photo-1589301277067-5c516e11eef0?w=600'
    WHEN ""Id"" IN (60) THEN 'https://images.unsplash.com/photo-1592521473069-d8b71fa5c9a0?w=600'
    WHEN ""Id"" IN (67) THEN 'https://images.unsplash.com/photo-1642019492077-999e179c188c?w=600'
    ELSE ""ImagePath""
END
WHERE ""Id"" BETWEEN 40 AND 67;
");
        }
    }
}
