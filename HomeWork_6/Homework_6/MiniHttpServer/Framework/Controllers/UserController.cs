using MyFirstHttp.Framework.Attributes;
using MyFirstHttp.Framework;
using MyFirstHttp.Framework.Results;

namespace MyFirstHttp.Framework.Controllers;

[Controller("user")]
public class UserController : BaseController
{
    [HttpGet("profile")]
    public IHttpResult Profile()
    {
        var user = new
        {
            Name = "Alex",
            Age = 20,
            Email = "alex@example.com"
        };

        var pageModel = new
        {
            Page = new
            {
                Title = "Профиль пользователя",
                Header = "Личный кабинет",
                Year = "2026",
                SiteName = "MySite"
            },
            User = user
        };

        return Page("User/Profile.html", pageModel);
    }
}
