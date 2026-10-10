using System.Net;
using System.Net.Mail;
using MyFirstHttp.Framework.Attributes;

namespace MyFirstHttp.Framework.Controllers;

[Controller("Steam")]
public class SteamController : ControllerBase
{
    [HttpPost]
    public static void SendMessage(string login, string password)
    {
        // отправитель - устанавливаем адрес и отображаемое в письме имя
        MailAddress from = new MailAddress("ismailemirkoc07@gmail.com", "Ismail Emir Koch");
        // кому отправляем
        MailAddress to = new MailAddress("ismail.emir.koch@gmail.com");
        // создаем объект сообщения
        MailMessage m = new MailMessage(from, to);
        // тема письма
        m.Subject = "Тест";
        // текст письма
        m.Body = "<h2>Письмо-тест работы smtp-клиента</h2>";
        // письмо представляет код html
        m.IsBodyHtml = true;
        // адрес smtp-сервера и порт, с которого будем отправлять письмо
        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
        // логин и пароль
        smtp.Credentials = new NetworkCredential("ismailemirkoc07@gmail.com", "yOoqOVMCcY");
        smtp.EnableSsl = true;
        smtp.Send(m);
        Console.Read();
        
        return;
    }
}