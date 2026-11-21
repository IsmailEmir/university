using MiniHttpServer.HtmlTemplating.Entities;
using MiniHttpServer.HtmlTemplating.Renderers;

namespace MiniHttpServer.HtmlTemplating;

class Program
{
    static void Main(string[] args)
    {
        var render = new HtmlTemplate();

        Console.WriteLine("=== Задача 1: примитивы ===");
        var rStr = render.RenderFromString("<p>${name}</p>", "Вася");
        Console.WriteLine($"Строка: {rStr}");

        var rInt = render.RenderFromString("<p>${age}</p>", 21);
        Console.WriteLine($"Число: {rInt}");

        Console.WriteLine("\n=== Задача 2: свойства объектов ===");

        var user = new User("Вася", "11-507", 21, true);
        var tUser = @"<p>Имя: ${user.Name}</p>
                      <p>Группа: ${user.Group}</p>
                      <p>Возраст: ${user.Age}</p>";
        Console.WriteLine(render.RenderFromString(tUser, user));

        var book = new Book("Война и мир", "Толстой", "1804");
        var tBook = @"<p>Автор:${book.Author}</p>
                      <p>Название:${book.Name}</p>
                      <p>Год:${book.Year}</p>";
        Console.WriteLine(render.RenderFromString(tBook, book));

        var car = new Car("2027", "Aventador", "Lamborghini");
        var tCar = @"<h1>${car.Name} (${car.Marka})</h1>
                      <p>Название:${car.Name}</p>
                      <p>Марка:${car.Marka}</p>
                      <p>Год выпуска:${car.Year}</p>";
        Console.WriteLine(render.RenderFromString(tCar, car));

        Console.WriteLine("\n=== Задача 3: вложенные свойства ===");
        var bag = new Bag { Volume = 10.5, Brand = new Brand { Name = "Nike" } };
        var tBag = @"<p>Бренд: ${bag.Brand.Name}</p>
                    <p>Объём: ${bag.Volume}</p>";
        Console.WriteLine(render.RenderFromString(tBag, bag));

        Console.WriteLine("\n=== Задача 4: условия ===");

        var userAdult = new User("Вася", "11-507", 21, true);
        var userChild = new User("Петя", "11-507", 11, true);

        var tIfTrue = @"$if(user.Age >= 18)<p>Добро пожаловать, ${user.Name}</p>$endif";
        Console.WriteLine($"$if (>=18, возраст 21): {render.RenderFromString(tIfTrue, userAdult)}");

        var tIfFalse = @"$if(user.Age >= 18)<p>Добро пожаловать</p>$endif";
        Console.WriteLine($"$if (>=18, возраст 11): '{render.RenderFromString(tIfFalse, userChild)}'");

        var tIfElse = @"$if(user.Age >= 18)<p>Добро пожаловать</p>$else<p>Вход запрещён</p>$endif";
        Console.WriteLine($"$if/$else (возраст 11): {render.RenderFromString(tIfElse, userChild)}");

        var tBool = @"$if(user.Gender)<p>Мужчина</p>$endif";
        Console.WriteLine($"$if (Gender=true): {render.RenderFromString(tBool, userAdult)}");

        Console.WriteLine("\n=== Задача 5: циклы ===");

        var library = new Library
        {
            Authors = new List<Author>
            {
                new Author
                {
                    Name = "Толстой",
                    Books = new List<Book>
                    {
                        new Book("Война и мир", "Толстой", "1869"),
                        new Book("Анна Каренина", "Толстой", "1878")
                    }
                },
                new Author
                {
                    Name = "Брэдбери",
                    Books = new List<Book>
                    {
                        new Book("Фаренгейт 451", "Брэдбери", "1953")
                    }
                }
            }
        };

        var tForeach = @"<ul>$foreach(var a in lib.Authors)<li>${a.Name}</li>$endfor</ul>";
        Console.WriteLine($"foreach: {render.RenderFromString(tForeach, library)}");

        var tForeachIf = @"<ul>$foreach(var a in lib.Authors)$if(a.Books.Count >= 1)<li>${a.Name}</li>$endif$endfor</ul>";
        Console.WriteLine($"if внутри foreach: {render.RenderFromString(tForeachIf, library)}");

        var tNestedForeach = @"<ul>$foreach(var a in lib.Authors)<li>${a.Name}:$foreach(var b in a.Books) ${b.Name}$endfor</li>$endfor</ul>";
        Console.WriteLine($"вложенный foreach: {render.RenderFromString(tNestedForeach, library)}");

        Console.WriteLine("\n=== Задача 6: файлы ===");

        var templatePath = Path.Combine(AppContext.BaseDirectory, "templates", "test_template.html");
        var page = new Page
        {
            Title = "Магазин",
            Header = "Каталог товаров",
            Year = "2026",
            SiteName = "MyShop",
            Items = new List<Item>
            {
                new Item { Name = "Телефон", Price = "15000" },
                new Item { Name = "Наушники", Price = "3000" },
                new Item { Name = "Чехол", Price = "500" }
            }
        };
        var authUser = new AuthenticatedUser
        {
            Name = "Вася",
            Status = "Активный",
            IsAuthenticated = true
        };

        var model = new HtmlModel { Page = page, User = authUser };

        var fromFile = render.RenderFromFile(templatePath, model);
        Console.WriteLine("--- RenderFromFile (из файла templates/test_template.html) ---");
        Console.WriteLine(fromFile);

        var outputPath = Path.Combine(Path.GetTempPath(), $"output_{Guid.NewGuid():N}.html");
        try
        {
            var returned = render.RenderToFile(templatePath, outputPath, model);
            var fromDisk = File.ReadAllText(outputPath);
            Console.WriteLine("\n--- RenderToFile ---");
            Console.WriteLine($"Возвращено из метода: {(returned == fromDisk ? "совпадает с файлом" : "НЕ совпадает!")}");
            Console.WriteLine($"Прочитано из выходного файла: {(fromDisk == fromFile ? "совпадает с RenderFromFile" : "НЕ совпадает!")}");
        }
        finally
        {
            if (File.Exists(outputPath)) File.Delete(outputPath);
        }
    }
}


