using System.Net;
using MyFirstHttp.Framework.Attributes;
using IO = System.IO;
using MyFirstHttp.Framework.Abstractions;
using MyFirstHttp.Framework.Results;
using MyFirstHttp.Framework.Utilities;

namespace MyFirstHttp.Framework.Controllers;

[Controller("homework")]
public class HomeworkController : ControllerBase
{
    private readonly EmailService _emailService;

    public HomeworkController()
    {
        _emailService = new EmailService();
    }
    
    [HttpGet("send")]
    public async Task<IActionResult> SendHomework()
    {
        string? toEmail = Query["to"];
        if (string.IsNullOrEmpty(toEmail))
        {
            return new TextResult($"[HomeworkController] Error. Please enter a valid email address.", HttpStatusCode.InternalServerError);
        }
        
        if (string.IsNullOrWhiteSpace(toEmail))
        {
            return new TextResult($"[HomeworkController] Error, type valid email", HttpStatusCode.BadRequest);
        }

        string zipPath = Path.Combine(AppContext.BaseDirectory, "homework.txt");

        if (!IO.File.Exists(zipPath))
        {
            return new TextResult($"[HomeworkController] Error. File was not found: {zipPath}",
                HttpStatusCode.InternalServerError);
        }

        bool success = await _emailService.SendHomeworkAsync(toEmail, zipPath);

        if (success)
        {
            return new TextResult($"Homework was sent successfully to {toEmail}", HttpStatusCode.OK);
        }

        return new TextResult($"Homework was not sent. Check logs.", HttpStatusCode.InternalServerError);
    }
}