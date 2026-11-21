using MyFirstHttp.Framework.Attributes;
using MyFirstHttp.Framework;
using MyFirstHttp.Framework.Results;

namespace MyFirstHttp.Framework.Controllers;

[Controller("test")]
public class TestController : BaseController
{
    [HttpGet("index")]
    public IHttpResult Index()
    {
        return Html("<h1>Hello from controller!</h1>");
    }

    [HttpGet("info")]
    public IHttpResult Info()
    {
        return Json(new
        {
            Id = 1,
            Message = "Hello World!",
            Success = true
        });
    }

    [HttpGet("requestinfo")]
    public IHttpResult RequestInfo()
    {
        if (Context == null)
        {
            return Json(new { error = "No context available" });
        }

        return Json(new
        {
            method = Context.Request.HttpMethod,
            path = Context.Request.Url?.AbsolutePath ?? "/",
            userAgent = Context.Request.Headers["User-Agent"]
        });
    }
}
