using System.Net;
using System.Net.Mail;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}


public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _fromEmail;

    public EmailService(IConfiguration configuration)
    {
        _fromEmail = configuration["EmailSettings:FromEmail"] ?? throw new Exception("FromEmail not configured");
        var smtpHost = configuration["EmailSettings:SmtpHost"] ?? throw new Exception("SmtpHost not configured");
        var smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"] ?? "587");
        var smtpUser = configuration["EmailSettings:SmtpUser"] ?? throw new Exception("SmtpUser not configured");
        var smtpPass = configuration["EmailSettings:SmtpPass"] ?? throw new Exception("SmtpPass not configured");

        _smtpClient = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var mailMessage = new MailMessage(_fromEmail, toEmail, subject, body);
        await _smtpClient.SendMailAsync(mailMessage);
    }
}