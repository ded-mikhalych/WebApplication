using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WebApplication.Data;

#nullable disable

namespace WebApplication.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260316230500_UseDistinctStepImages")]
    public partial class UseDistinctStepImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE ""Recipes""
SET ""ImageFileName"" = CASE ""Id""
    WHEN 7 THEN 'https://upload.wikimedia.org/wikipedia/commons/0/0a/Traditional_Uzbek_Pilaf_in_a_Cauldron_on_a_Potbelly_Stove_Moscow_Region.jpg'
    WHEN 8 THEN 'https://upload.wikimedia.org/wikipedia/commons/9/9c/Tom_Yum_Soup.JPG'
    WHEN 9 THEN 'https://upload.wikimedia.org/wikipedia/commons/2/27/Ratatouille.jpg'
    WHEN 10 THEN 'https://upload.wikimedia.org/wikipedia/commons/3/32/Beef_Stroganoff-02.jpg'
    ELSE ""ImageFileName""
END
WHERE ""Id"" BETWEEN 7 AND 10;
");

            migrationBuilder.Sql(@"
UPDATE ""RecipeSteps""
SET ""Description"" = CASE ""Id""
    WHEN 40 THEN 'Переберите рис, затем промывайте его в нескольких водах до прозрачности и замочите на 25-30 минут.'
    WHEN 41 THEN 'Баранину нарежьте средними кубиками, лук полукольцами, морковь длинной соломкой примерно одинаковой толщины.'
    WHEN 42 THEN 'Сильно разогрейте казан с маслом и обжарьте мясо партиями до уверенной румяной корочки.'
    WHEN 43 THEN 'Добавьте лук к мясу, готовьте до золотистого оттенка и сладкого аромата, не пережигая.'
    WHEN 44 THEN 'Всыпьте морковь, перемешайте и томите 8-10 минут, затем добавьте зиру и барбарис для аромата.'
    WHEN 45 THEN 'Влейте горячую воду, чтобы покрыть содержимое, посолите и готовьте зирвак на умеренном огне около 30 минут.'
    WHEN 46 THEN 'Слейте рис, распределите его ровным слоем, аккуратно долейте воду на 1 см выше риса и варите без крышки.'
    WHEN 47 THEN 'Когда жидкость уйдет, вставьте головки чеснока, соберите рис горкой, накройте и томите 20 минут до готовности.'

    WHEN 48 THEN 'Очистите креветки, удалите кишечную вену, лемонграсс слегка раздавите тыльной стороной ножа.'
    WHEN 49 THEN 'Доведите бульон до мягкого кипения, добавьте лемонграсс и листья каффир-лайма, прогрейте 3-4 минуты.'
    WHEN 50 THEN 'Растворите пасту том ям в бульоне, чтобы вкус распределился равномерно по всей основе супа.'
    WHEN 51 THEN 'Добавьте грибы и готовьте несколько минут до мягкости, сохраняя умеренный огонь.'
    WHEN 52 THEN 'Влейте кокосовое молоко и прогрейте суп почти до кипения, но не давайте ему бурно кипеть.'
    WHEN 53 THEN 'Положите креветки и варите 2-3 минуты, пока они не станут розовыми и упругими.'
    WHEN 54 THEN 'Отрегулируйте баланс вкуса рыбным соусом и соком лайма: солено, кисло, слегка остро.'
    WHEN 55 THEN 'Снимите с огня, добавьте кинзу и подавайте сразу, пока суп горячий и ароматный.'

    WHEN 56 THEN 'Нарежьте баклажан, кабачок и томаты тонкими кружками; баклажан при необходимости слегка подсолите и обсушите.'
    WHEN 57 THEN 'Для соуса обжарьте лук и перец, добавьте часть томатов и тушите 10 минут до густой овощной основы.'
    WHEN 58 THEN 'Выложите соус на дно формы и разложите овощи рядами или спиралью, чередуя цвета.'
    WHEN 59 THEN 'Сбрызните оливковым маслом, посыпьте чесноком, тимьяном и базиликом, слегка подсолите.'
    WHEN 60 THEN 'Накройте форму фольгой и запекайте при 180C 30 минут, затем снимите фольгу и готовьте еще 10 минут.'

    WHEN 61 THEN 'Нарежьте говядину тонкими полосками поперек волокон, промокните бумажным полотенцем.'
    WHEN 62 THEN 'Слегка обваляйте мясо в муке и быстро обжарьте на сильном огне до легкой корочки, не пересушивая.'
    WHEN 63 THEN 'Отдельно обжарьте лук и грибы до выпаривания влаги и мягкой карамелизации.'
    WHEN 64 THEN 'Верните мясо в сковороду к луку и грибам, добавьте горчицу, соль и свежемолотый перец.'
    WHEN 65 THEN 'Влейте сливки и добавьте сметану, аккуратно перемешайте и доведите соус до однородности.'
    WHEN 66 THEN 'Тушите 8-10 минут на слабом огне до мягкости мяса и нужной густоты соуса.'
    WHEN 67 THEN 'Подавайте горячим с картофельным пюре, рисом или пастой; сверху можно добавить немного зелени.'
    ELSE ""Description""
END,
""ImagePath"" = CASE ""Id""
    WHEN 40 THEN 'https://upload.wikimedia.org/wikipedia/commons/f/f8/Cook_and_Samarkandian_pilaf_%28palov%29_with_linseed_oil.jpg'
    WHEN 41 THEN 'https://upload.wikimedia.org/wikipedia/commons/1/10/Khiva%2C_Pilaf.jpeg'
    WHEN 42 THEN 'https://upload.wikimedia.org/wikipedia/commons/7/71/Making_Zirvak_Rich_Stew_of_Meat_and_Vegetables_and_Spices_for_Uzbek_Pilaf_with_Quinces_in_Newton_Massachusetts.jpg'
    WHEN 43 THEN 'https://upload.wikimedia.org/wikipedia/commons/c/c7/Pilaf_of_Uzbekistan%2C_Yerevan.jpg'
    WHEN 44 THEN 'https://upload.wikimedia.org/wikipedia/commons/5/5c/Samarkand_Zigir-pilaf.jpg'
    WHEN 45 THEN 'https://upload.wikimedia.org/wikipedia/commons/6/61/Samarkand_flax_pilaf.jpg'
    WHEN 46 THEN 'https://upload.wikimedia.org/wikipedia/commons/f/f2/Samarqand_Zig%27ir_Oshi_pilaf.jpg'
    WHEN 47 THEN 'https://upload.wikimedia.org/wikipedia/commons/5/50/Traditional_Lyagan_Large_Flat_Plate_Tableware_Dish_on_which_Uzbek_Pilaf_Served_Moscow_Region.jpg'

    WHEN 48 THEN 'https://upload.wikimedia.org/wikipedia/commons/b/b1/Mushrooms_Tom_Yum%2C_Duck_Pa-naeng%2C_and_Thai_Jasmine_Rice_-_Sawadee_Thai_Restaurant_2024-10-05.jpg'
    WHEN 49 THEN 'https://upload.wikimedia.org/wikipedia/commons/5/5e/Shrimp_Tom_yum_soup_from_a_Thai_restaurant_in_Delray_Beach%2C_Florida.jpg'
    WHEN 50 THEN 'https://upload.wikimedia.org/wikipedia/commons/6/6d/Tom-yum-het-saam-yaang.JPG'
    WHEN 51 THEN 'https://upload.wikimedia.org/wikipedia/commons/8/8a/Tom_Yum_%28Hot_%26_Sour_Soup%29_with_Tofu_-_Thai_Pad_Thai_2025-12-03.jpg'
    WHEN 52 THEN 'https://upload.wikimedia.org/wikipedia/commons/1/15/Tom_Yum_Goong_Noodle_Soup_-_Nok_Nok_Kitchen_at_The_Cow_2024-03-28.jpg'
    WHEN 53 THEN 'https://upload.wikimedia.org/wikipedia/commons/a/a5/Tom_Yum_Ingredients_%284297200768%29.jpg'
    WHEN 54 THEN 'https://upload.wikimedia.org/wikipedia/commons/d/dc/Tom_Yum_Koong_Soup_with_Prawn_and_Straw_Mushroom.jpg'
    WHEN 55 THEN 'https://upload.wikimedia.org/wikipedia/commons/8/88/Tom_Yum_Soup_%284296454661%29.jpg'

    WHEN 56 THEN 'https://upload.wikimedia.org/wikipedia/commons/7/7a/20170329_ratatouille-fin-cuisson.jpg'
    WHEN 57 THEN 'https://upload.wikimedia.org/wikipedia/commons/4/49/Payam_ratatoie.jpg'
    WHEN 58 THEN 'https://upload.wikimedia.org/wikipedia/commons/0/03/Ratatouille-Dish.jpg'
    WHEN 59 THEN 'https://upload.wikimedia.org/wikipedia/commons/8/8a/Ratatouille02.jpg'
    WHEN 60 THEN 'https://upload.wikimedia.org/wikipedia/commons/6/67/Ratatouille_-_Flickr_-_odako1.jpg'

    WHEN 61 THEN 'https://upload.wikimedia.org/wikipedia/commons/0/0f/-2019-12-10_Beef_Stroganoff%2C_Trimingham_%281%29.JPG'
    WHEN 62 THEN 'https://upload.wikimedia.org/wikipedia/commons/5/50/-2019-12-10_Beef_Stroganoff%2C_Trimingham_%282%29.JPG'
    WHEN 63 THEN 'https://upload.wikimedia.org/wikipedia/commons/c/c7/-2020-09-14_Beef_stroganoff%2C_Trimingham.JPG'
    WHEN 64 THEN 'https://upload.wikimedia.org/wikipedia/commons/5/5f/Beef_Stroganoff-02_cropped.jpg'
    WHEN 65 THEN 'https://upload.wikimedia.org/wikipedia/commons/6/67/Beef_Stroganoff-03.jpg'
    WHEN 66 THEN 'https://upload.wikimedia.org/wikipedia/commons/4/4e/Beef_Stroganoff.jpg'
    WHEN 67 THEN 'https://upload.wikimedia.org/wikipedia/commons/6/67/Beef_Stroganoff_2.jpg'
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
    }
}
