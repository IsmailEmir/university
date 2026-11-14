using MyFirstHttp.Framework.Attributes;

namespace MyFirstHttp.Framework.Controllers;

[Controller("auth")]
public class Auth : ControllerBase
{
    public Auth()
    {
    }

    [HttpPost]
    public string Login(string email, string password)
    {
        Console.WriteLine($"[Данные пользователя]\nПочта: {email}\nПароль: {password}");

        return $"Успешный вход для {email}";
    }
}
