using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace FinanzApp.Web.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly string _server;
    private readonly int _port;
    private readonly string _senderName;
    private readonly string _senderEmail;
    private readonly string _password;

    public SmtpEmailSender(IConfiguration configuration)
    {
        var section = configuration.GetSection("SmtpSettings");
        _server = section["Server"] ?? "smtp.gmail.com";
        _port = int.TryParse(section["Port"], out var port) ? port : 587;
        _senderName = section["SenderName"] ?? "FinanzApp";
        _senderEmail = section["SenderEmail"] ?? string.Empty;
        _password = section["Password"] ?? string.Empty;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_senderEmail, _senderName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            using var smtpClient = new SmtpClient(_server)
            {
                Port = _port,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_senderEmail, _password),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine($"[SMTP] Correo enviado a {email} (asunto: {subject}).");
        }
        catch (SmtpException ex)
        {
            Console.WriteLine($"[ERROR SMTP]: {ex.Message} | StatusCode: {ex.StatusCode}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[INNER]: {ex.InnerException.Message}");
            }
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR GENERAL SMTP]: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[INNER]: {ex.InnerException.Message}");
            }
            throw;
        }
    }
}
