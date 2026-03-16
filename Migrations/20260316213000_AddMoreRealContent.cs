using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WebApplication.Data;

#nullable disable

namespace WebApplication.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260316213000_AddMoreRealContent")]
    public partial class AddMoreRealContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO ""Recipes"" (""Id"", ""Name"", ""Slug"", ""Description"", ""Cuisine"", ""Difficulty"", ""ImageFileName"", ""IsFavorite"", ""CookingTime"", ""CategoryId"", ""CreatedAt"", ""RatingSum"", ""RatingCount"")
VALUES
    (7, 'Узбекский плов', 'plov', 'Классический плов с бараниной, рисом и морковью в казане', 'Узбекская', 2, 'https://upload.wikimedia.org/wikipedia/commons/0/0a/Traditional_Uzbek_Pilaf_in_a_Cauldron_on_a_Potbelly_Stove_Moscow_Region.jpg', FALSE, 90, 3, TIMESTAMPTZ '2026-03-06 00:00:00+00', 0, 0),
    (8, 'Том ям с креветками', 'tom-yum', 'Пряный тайский суп на кокосовом молоке с креветками и лаймом', 'Тайская', 3, 'https://upload.wikimedia.org/wikipedia/commons/9/9c/Tom_Yum_Soup.JPG', FALSE, 45, 3, TIMESTAMPTZ '2026-03-08 00:00:00+00', 0, 0),
    (9, 'Рататуй', 'ratatouille', 'Французское овощное рагу из баклажанов, кабачков и томатов', 'Французская', 2, 'https://upload.wikimedia.org/wikipedia/commons/2/27/Ratatouille.jpg', FALSE, 60, 2, TIMESTAMPTZ '2026-03-09 00:00:00+00', 0, 0),
    (10, 'Бефстроганов', 'beef-stroganoff', 'Говядина в сливочно-сметанном соусе с луком и грибами', 'Русская', 2, 'https://upload.wikimedia.org/wikipedia/commons/3/32/Beef_Stroganoff-02.jpg', FALSE, 50, 1, TIMESTAMPTZ '2026-03-10 00:00:00+00', 0, 0)
ON CONFLICT (""Id"") DO NOTHING;
");

            migrationBuilder.Sql(@"
INSERT INTO ""RecipeIngredients"" (""Id"", ""RecipeId"", ""DisplayText"", ""SortOrder"")
VALUES
    (46, 7, 'Рис девзира — 700 г', 1),
    (47, 7, 'Баранина — 700 г', 2),
    (48, 7, 'Морковь — 700 г', 3),
    (49, 7, 'Лук — 300 г', 4),
    (50, 7, 'Чеснок — 2 головки', 5),
    (51, 7, 'Зира — 1.5 ч.л.', 6),
    (52, 7, 'Барбарис — 1 ст.л.', 7),
    (53, 7, 'Растительное масло — 120 мл', 8),
    (54, 7, 'Соль — по вкусу', 9),

    (55, 8, 'Креветки — 400 г', 1),
    (56, 8, 'Кокосовое молоко — 400 мл', 2),
    (57, 8, 'Куриный бульон — 600 мл', 3),
    (58, 8, 'Паста том ям — 2 ст.л.', 4),
    (59, 8, 'Лемонграсс — 2 стебля', 5),
    (60, 8, 'Листья каффир-лайма — 4 шт.', 6),
    (61, 8, 'Шампиньоны — 200 г', 7),
    (62, 8, 'Рыбный соус — 2 ст.л.', 8),
    (63, 8, 'Сок лайма — 2 ст.л.', 9),

    (64, 9, 'Баклажан — 1 крупный', 1),
    (65, 9, 'Кабачок — 1 крупный', 2),
    (66, 9, 'Помидоры — 5 шт.', 3),
    (67, 9, 'Болгарский перец — 2 шт.', 4),
    (68, 9, 'Лук — 1 шт.', 5),
    (69, 9, 'Чеснок — 3 зубчика', 6),
    (70, 9, 'Оливковое масло — 3 ст.л.', 7),
    (71, 9, 'Тимьян и базилик — по вкусу', 8),

    (72, 10, 'Говяжья вырезка — 600 г', 1),
    (73, 10, 'Лук — 2 шт.', 2),
    (74, 10, 'Шампиньоны — 250 г', 3),
    (75, 10, 'Сметана — 250 г', 4),
    (76, 10, 'Сливки 20% — 150 мл', 5),
    (77, 10, 'Горчица — 1 ч.л.', 6),
    (78, 10, 'Мука — 1 ст.л.', 7),
    (79, 10, 'Соль и перец — по вкусу', 8)
ON CONFLICT (""Id"") DO NOTHING;
");

            migrationBuilder.Sql(@"
INSERT INTO ""RecipeSteps"" (""Id"", ""RecipeId"", ""StepNumber"", ""Description"", ""ImagePath"")
VALUES
    (40, 7, 1, 'Переберите рис, затем промывайте его в нескольких водах до прозрачности и замочите на 25-30 минут.', 'https://upload.wikimedia.org/wikipedia/commons/f/f8/Cook_and_Samarkandian_pilaf_%28palov%29_with_linseed_oil.jpg'),
    (41, 7, 2, 'Баранину нарежьте средними кубиками, лук полукольцами, морковь длинной соломкой примерно одинаковой толщины.', 'https://upload.wikimedia.org/wikipedia/commons/1/10/Khiva%2C_Pilaf.jpeg'),
    (42, 7, 3, 'Сильно разогрейте казан с маслом и обжарьте мясо партиями до уверенной румяной корочки.', 'https://upload.wikimedia.org/wikipedia/commons/7/71/Making_Zirvak_Rich_Stew_of_Meat_and_Vegetables_and_Spices_for_Uzbek_Pilaf_with_Quinces_in_Newton_Massachusetts.jpg'),
    (43, 7, 4, 'Добавьте лук к мясу, готовьте до золотистого оттенка и сладкого аромата, не пережигая.', 'https://upload.wikimedia.org/wikipedia/commons/c/c7/Pilaf_of_Uzbekistan%2C_Yerevan.jpg'),
    (44, 7, 5, 'Всыпьте морковь, перемешайте и томите 8-10 минут, затем добавьте зиру и барбарис для аромата.', 'https://upload.wikimedia.org/wikipedia/commons/5/5c/Samarkand_Zigir-pilaf.jpg'),
    (45, 7, 6, 'Влейте горячую воду, чтобы покрыть содержимое, посолите и готовьте зирвак на умеренном огне около 30 минут.', 'https://upload.wikimedia.org/wikipedia/commons/6/61/Samarkand_flax_pilaf.jpg'),
    (46, 7, 7, 'Слейте рис, распределите его ровным слоем, аккуратно долейте воду на 1 см выше риса и варите без крышки.', 'https://upload.wikimedia.org/wikipedia/commons/f/f2/Samarqand_Zig%27ir_Oshi_pilaf.jpg'),
    (47, 7, 8, 'Когда жидкость уйдет, вставьте головки чеснока, соберите рис горкой, накройте и томите 20 минут до готовности.', 'https://upload.wikimedia.org/wikipedia/commons/5/50/Traditional_Lyagan_Large_Flat_Plate_Tableware_Dish_on_which_Uzbek_Pilaf_Served_Moscow_Region.jpg'),

    (48, 8, 1, 'Очистите креветки, удалите кишечную вену, лемонграсс слегка раздавите тыльной стороной ножа.', 'https://upload.wikimedia.org/wikipedia/commons/b/b1/Mushrooms_Tom_Yum%2C_Duck_Pa-naeng%2C_and_Thai_Jasmine_Rice_-_Sawadee_Thai_Restaurant_2024-10-05.jpg'),
    (49, 8, 2, 'Доведите бульон до мягкого кипения, добавьте лемонграсс и листья каффир-лайма, прогрейте 3-4 минуты.', 'https://upload.wikimedia.org/wikipedia/commons/5/5e/Shrimp_Tom_yum_soup_from_a_Thai_restaurant_in_Delray_Beach%2C_Florida.jpg'),
    (50, 8, 3, 'Растворите пасту том ям в бульоне, чтобы вкус распределился равномерно по всей основе супа.', 'https://upload.wikimedia.org/wikipedia/commons/6/6d/Tom-yum-het-saam-yaang.JPG'),
    (51, 8, 4, 'Добавьте грибы и готовьте несколько минут до мягкости, сохраняя умеренный огонь.', 'https://upload.wikimedia.org/wikipedia/commons/8/8a/Tom_Yum_%28Hot_%26_Sour_Soup%29_with_Tofu_-_Thai_Pad_Thai_2025-12-03.jpg'),
    (52, 8, 5, 'Влейте кокосовое молоко и прогрейте суп почти до кипения, но не давайте ему бурно кипеть.', 'https://upload.wikimedia.org/wikipedia/commons/1/15/Tom_Yum_Goong_Noodle_Soup_-_Nok_Nok_Kitchen_at_The_Cow_2024-03-28.jpg'),
    (53, 8, 6, 'Положите креветки и варите 2-3 минуты, пока они не станут розовыми и упругими.', 'https://upload.wikimedia.org/wikipedia/commons/a/a5/Tom_Yum_Ingredients_%284297200768%29.jpg'),
    (54, 8, 7, 'Отрегулируйте баланс вкуса рыбным соусом и соком лайма: солено, кисло, слегка остро.', 'https://upload.wikimedia.org/wikipedia/commons/d/dc/Tom_Yum_Koong_Soup_with_Prawn_and_Straw_Mushroom.jpg'),
    (55, 8, 8, 'Снимите с огня, добавьте кинзу и подавайте сразу, пока суп горячий и ароматный.', 'https://upload.wikimedia.org/wikipedia/commons/8/88/Tom_Yum_Soup_%284296454661%29.jpg'),

    (56, 9, 1, 'Нарежьте баклажан, кабачок и томаты тонкими кружками; баклажан при необходимости слегка подсолите и обсушите.', 'https://upload.wikimedia.org/wikipedia/commons/7/7a/20170329_ratatouille-fin-cuisson.jpg'),
    (57, 9, 2, 'Для соуса обжарьте лук и перец, добавьте часть томатов и тушите 10 минут до густой овощной основы.', 'https://upload.wikimedia.org/wikipedia/commons/4/49/Payam_ratatoie.jpg'),
    (58, 9, 3, 'Выложите соус на дно формы и разложите овощи рядами или спиралью, чередуя цвета.', 'https://upload.wikimedia.org/wikipedia/commons/0/03/Ratatouille-Dish.jpg'),
    (59, 9, 4, 'Сбрызните оливковым маслом, посыпьте чесноком, тимьяном и базиликом, слегка подсолите.', 'https://upload.wikimedia.org/wikipedia/commons/8/8a/Ratatouille02.jpg'),
    (60, 9, 5, 'Накройте форму фольгой и запекайте при 180C 30 минут, затем снимите фольгу и готовьте еще 10 минут.', 'https://upload.wikimedia.org/wikipedia/commons/6/67/Ratatouille_-_Flickr_-_odako1.jpg'),

    (61, 10, 1, 'Нарежьте говядину тонкими полосками поперек волокон, промокните бумажным полотенцем.', 'https://upload.wikimedia.org/wikipedia/commons/0/0f/-2019-12-10_Beef_Stroganoff%2C_Trimingham_%281%29.JPG'),
    (62, 10, 2, 'Слегка обваляйте мясо в муке и быстро обжарьте на сильном огне до легкой корочки, не пересушивая.', 'https://upload.wikimedia.org/wikipedia/commons/5/50/-2019-12-10_Beef_Stroganoff%2C_Trimingham_%282%29.JPG'),
    (63, 10, 3, 'Отдельно обжарьте лук и грибы до выпаривания влаги и мягкой карамелизации.', 'https://upload.wikimedia.org/wikipedia/commons/c/c7/-2020-09-14_Beef_stroganoff%2C_Trimingham.JPG'),
    (64, 10, 4, 'Верните мясо в сковороду к луку и грибам, добавьте горчицу, соль и свежемолотый перец.', 'https://upload.wikimedia.org/wikipedia/commons/5/5f/Beef_Stroganoff-02_cropped.jpg'),
    (65, 10, 5, 'Влейте сливки и добавьте сметану, аккуратно перемешайте и доведите соус до однородности.', 'https://upload.wikimedia.org/wikipedia/commons/6/67/Beef_Stroganoff-03.jpg'),
    (66, 10, 6, 'Тушите 8-10 минут на слабом огне до мягкости мяса и нужной густоты соуса.', 'https://upload.wikimedia.org/wikipedia/commons/4/4e/Beef_Stroganoff.jpg'),
    (67, 10, 7, 'Подавайте горячим с картофельным пюре, рисом или пастой; сверху можно добавить немного зелени.', 'https://upload.wikimedia.org/wikipedia/commons/6/67/Beef_Stroganoff_2.jpg')
ON CONFLICT (""Id"") DO NOTHING;
");

            migrationBuilder.Sql(@"
INSERT INTO ""News"" (""Id"", ""Title"", ""Summary"", ""ContentHtml"", ""ImageFileName"", ""CreatedAt"")
VALUES
    (5, 'Средиземноморская диета и здоровье сердца', 'Кардиологи подтверждают пользу рациона с овощами, рыбой и оливковым маслом.', '<p>Систематические обзоры показывают, что средиземноморская модель питания связана с более низким риском сердечно-сосудистых событий. В рационе преобладают овощи, бобовые, цельные злаки, рыба и оливковое масло.</p><p>Эксперты подчеркивают, что ключевой эффект достигается не отдельным продуктом, а устойчивым режимом питания и умеренной физической активностью.</p>', 'news1.png', TIMESTAMPTZ '2026-03-11 00:00:00+00'),
    (6, 'Ферментированные продукты и микробиота', 'Йогурт, кефир и квашеные овощи могут поддерживать разнообразие кишечной микрофлоры.', '<p>Диетологи отмечают, что регулярное употребление ферментированных продуктов способно положительно влиять на микробиоту кишечника. Наиболее часто исследуются кефир, натуральный йогурт, квашеная капуста и кимчи.</p><p>При этом специалисты напоминают о важности состава: продукты с избытком соли или сахара могут нивелировать потенциальную пользу.</p>', 'news2.png', TIMESTAMPTZ '2026-03-12 00:00:00+00'),
    (7, 'Почему важно есть больше бобовых', 'Фасоль, чечевица и нут помогают добирать белок и пищевые волокна.', '<p>Бобовые остаются одним из самых доступных источников растительного белка и клетчатки. По данным профильных рекомендаций по питанию, их регулярное включение в меню может улучшать липидный профиль и насыщаемость.</p><p>Шеф-повара советуют начинать с простых блюд: чечевичного супа, хумуса и салатов с фасолью.</p>', 'news3.png', TIMESTAMPTZ '2026-03-13 00:00:00+00'),
    (8, 'Цельнозерновые продукты против скачков сахара', 'Замена рафинированной муки на цельнозерновую снижает гликемическую нагрузку рациона.', '<p>Нутрициологи рекомендуют чаще выбирать цельнозерновой хлеб, крупы и макароны из твердых сортов пшеницы. Такие продукты содержат больше пищевых волокон и медленнее повышают уровень глюкозы в крови.</p><p>Для постепенного перехода достаточно заменить хотя бы половину привычных гарниров на цельнозерновые аналоги.</p>', 'news4.jpg', TIMESTAMPTZ '2026-03-14 00:00:00+00'),
    (9, 'Как безопасно готовить рыбу дома', 'Температурный контроль и правильное хранение снижают риск пищевых инфекций.', '<p>Санитарные рекомендации напоминают: сырую рыбу нужно хранить отдельно от готовых продуктов и использовать отдельные разделочные поверхности. Термическая обработка до полной готовности значительно снижает микробиологические риски.</p><p>Также важно не размораживать рыбу при комнатной температуре: лучше делать это в холодильнике.</p>', 'news1.png', TIMESTAMPTZ '2026-03-15 00:00:00+00'),
    (10, 'Сезонные овощи весной: что выбирать', 'Диетологи составили список доступных овощей для сбалансированного меню весной.', '<p>Весной специалисты советуют делать акцент на капусте, моркови, свекле, зелени и замороженных овощных смесях высокого качества. Такой набор помогает закрыть потребность в клетчатке и микронутриентах.</p><p>Для практичного меню подойдут овощные супы, запеканки и теплые салаты с бобовыми.</p>', 'news2.png', TIMESTAMPTZ '2026-03-16 00:00:00+00')
ON CONFLICT (""Id"") DO NOTHING;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM ""RecipeSteps"" WHERE ""Id"" BETWEEN 40 AND 67;");
            migrationBuilder.Sql(@"DELETE FROM ""RecipeIngredients"" WHERE ""Id"" BETWEEN 46 AND 79;");
            migrationBuilder.Sql(@"DELETE FROM ""Recipes"" WHERE ""Id"" BETWEEN 7 AND 10;");
            migrationBuilder.Sql(@"DELETE FROM ""News"" WHERE ""Id"" BETWEEN 5 AND 10;");
        }
    }
}
