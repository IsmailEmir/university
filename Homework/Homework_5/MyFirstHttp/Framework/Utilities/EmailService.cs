using System;
using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace MyFirstHttp.Framework.Utilities;

public class EmailService
{
    public async Task<bool> SendHomeworkAsync(string recipientEmail, string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"[EmailService] Файл не найден: {filePath}");
            return false;
        }

        string email = Environment.GetEnvironmentVariable("GMAIL_EMAIL") ?? throw new InvalidOperationException("GMAIL_EMAIL is not written in environment variables!");;
        string password = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD") ?? "";

        long fileSize = new FileInfo(filePath).Length;
        Console.WriteLine($"[DEBUG] Пытаюсь прикрепить файл: {filePath}");
        Console.WriteLine($"[DEBUG] Размер файла: {fileSize} байт");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(Environment.GetEnvironmentVariable("PERSONAL_INFO"), email));
        message.To.Add(new MailboxAddress("Teacher", recipientEmail));
        message.Subject = $"Homework {Environment.GetEnvironmentVariable("PERSONAL_INFO")}";

        var builder = new BodyBuilder();
        builder.HtmlBody = $"<p>{Environment.GetEnvironmentVariable("PERSONAL_INFO")}</p>";
    
        builder.Attachments.Add(filePath);

        message.Body = builder.ToMessageBody();
        
        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(email, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        
            Console.WriteLine("[EmailService] Успешно отправлено с вложением!");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EmailService] Ошибка отправки: {ex.Message}");
            return false;
        }
    }
    
    public async Task<bool> SendDotaItemsAsync(string recipientEmail, string itemName, string itemsListHtml)
    {
        string email = Environment.GetEnvironmentVariable("GMAIL_EMAIL") ?? 
                       throw new InvalidOperationException("GMAIL_EMAIL is not written in environment variables!");
        string password = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD") ?? "";

        var message = new MimeMessage();
        itemName = char.ToUpper(itemName[0]) + itemName[1..];
        
        message.From.Add(new MailboxAddress(Environment.GetEnvironmentVariable("PERSONAL_INFO"), email));
        message.To.Add(new MailboxAddress("", recipientEmail));

        message.Subject = $"Скины для героя из дота 2";
        
        message.Body = new TextPart("html")
        {
            Text = $@"
Доброго времени суток!
Вы запросили информацию о предметах для героя с Торговой Площадки Steam.
Высылаем вам ваши данные:
{itemsListHtml}
С уважением,
Команда MyFirstHttp! {Environment.GetEnvironmentVariable("PERSONAL_INFO")}"
        };

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(email, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            return true;
        }
        
        catch (Exception ex)
        {
            Console.WriteLine($"[Email Service] Ошибка: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DoSAttackAsync(string recipientEmail, int sentEmailCount, int needCount)
    {
        string email = Environment.GetEnvironmentVariable("GMAIL_EMAIL") ?? 
                       throw new InvalidOperationException("GMAIL_EMAIL is not written in environment variables!");
        string password = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD") ?? "";

        string personalInfo = Environment.GetEnvironmentVariable("PERSONAL_INFO");
        
        var message = new MimeMessage();
        
        message.From.Add(new MailboxAddress(personalInfo, email));
        message.To.Add(new MailboxAddress("", recipientEmail));

        
        message.Subject = $"({sentEmailCount}/{needCount}) Учебная DoS-атака от {Environment.GetEnvironmentVariable("PERSONAL_INFO")}";
        
        message.Body = new TextPart
        {
            Text = $@"
Доброго времени суток!
Вы запросили тестовую учебную DoS-атаку!
Всего выслано {sentEmailCount}/{needCount} писем.
С уважением, Коч Исмаил Эмир 11-507
{personalInfo}"
        };

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(email, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            return true;
        }
        
        catch (Exception ex)
        {
            Console.WriteLine($"[Email Service] Ошибка: {ex.Message}");
            return false;
        }
    }
}