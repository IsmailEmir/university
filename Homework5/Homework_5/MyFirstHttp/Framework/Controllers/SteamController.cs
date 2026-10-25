using System.Net;
using System.Text.RegularExpressions;
using MyFirstHttp.Framework.Abstractions;
using MyFirstHttp.Framework.Attributes;
using MyFirstHttp.Framework.Utilities;

namespace MyFirstHttp.Framework.Controllers;

[Controller("steam")]
public class SteamController : ControllerBase
{
    private static readonly EmailService _emailService = new EmailService();
    private static readonly SteamMarketService _steamMarketService = new SteamMarketService();
    
    public SteamController(){}
    
    [HttpGet("send")]
    public async Task<IActionResult> SendMessage()
    {
        string toEmail = Query["to"];
        string hero = Query["hero"]?.Trim() ?? "";
        
        if (string.IsNullOrEmpty(toEmail) || string.IsNullOrEmpty(hero))
        {
            return Text("Invalid email data", HttpStatusCode.BadRequest);
        }

        if (!IsValidEmail(toEmail))
        {
            return Text("Invalid email format", HttpStatusCode.BadRequest);
        }

        hero = Regex.Replace(hero, @"[^a-zA-Zа-яА-Я0-9\s\-]", "");

        try
        {
            string itemsHtml = await _steamMarketService.GetHeroesHtmlAsync(hero);
            
            bool success = await _emailService.SendDotaItemsAsync(toEmail, hero, itemsHtml);

            if (success)
            {
                return Text($"Successful! List of items of {hero} were sent to {toEmail}", HttpStatusCode.OK);
            }
            else
            {
                return Text($"Email Service problem. Check the logs", HttpStatusCode.InternalServerError);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SteamController] Something went wrong: {ex.Message}");
            return Text(ex.Message, HttpStatusCode.InternalServerError);
        }
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