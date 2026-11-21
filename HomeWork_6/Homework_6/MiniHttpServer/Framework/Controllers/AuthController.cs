using MyFirstHttp.Framework.Attributes;
using MyFirstHttp.Framework.Results;

namespace MyFirstHttp.Framework.Controllers;

[Controller("auth")]
public class Auth : BaseController
{
    public Auth()
    {
    }

    [HttpPost]
    public IHttpResult Login(string email, string password)
    {
        Console.WriteLine($"\n[Данные пользователя]\nПочта: {email}\nПароль: {password}");

        try
        {
            return new JsonResult(new
            {
                success = true,
                message = "Сообщение отправлено"
            });
        }
        catch (Exception)
        {
            return new JsonResult(new
            {
                success = false,
                message = "Не удалось отправить сообщение"
            });
        }
    }
}
