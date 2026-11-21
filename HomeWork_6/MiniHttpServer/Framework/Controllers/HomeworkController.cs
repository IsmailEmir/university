using System.Net;
using MyFirstHttp.Framework;
using MyFirstHttp.Framework.Attributes;
using MyFirstHttp.Framework.Results;
using MyFirstHttp.Framework.Utilities;
using IO = System.IO;

namespace MyFirstHttp.Framework.Controllers;

[Controller("homework")]
public class HomeworkController : BaseController
{
    private readonly EmailService _emailService;

    public HomeworkController()
    {
        _emailService = new EmailService();
    }

    [HttpGet("send")]
    public async Task<IHttpResult> SendHomework()
    {
        string? toEmail = Query["to"];
        if (string.IsNullOrEmpty(toEmail))
        {
            return Text("[HomeworkController] Error. Please enter a valid email address.", HttpStatusCode.InternalServerError);
        }

        if (string.IsNullOrWhiteSpace(toEmail))
        {
            return Text("[HomeworkController] Error, type valid email", HttpStatusCode.BadRequest);
        }

        string zipPath = Path.Combine(AppContext.BaseDirectory, "homework.txt");

        if (!IO.File.Exists(zipPath))
        {
            return Text($"[HomeworkController] Error. File was not found: {zipPath}", HttpStatusCode.InternalServerError);
        }

        bool success = await _emailService.SendHomeworkAsync(toEmail, zipPath);

        if (success)
        {
            return Text("Homework was sent successfully to " + toEmail, HttpStatusCode.OK);
        }

        return Text("Homework was not sent. Check logs.", HttpStatusCode.InternalServerError);
    }
}
