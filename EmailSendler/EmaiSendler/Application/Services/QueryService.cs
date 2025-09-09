using System.Net;
using System.Net.Mail;
using Application.Interfaces;
using Application.Options;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class QueryService(IOptions<EmailOptions> options) : IQueryService
{
    private readonly EmailOptions _options = options.Value;

    public async Task SendEmailMessageAsync(string toEmail, string subject, string body)
    {
        if (string.IsNullOrEmpty(toEmail)) throw new ArgumentNullException(nameof(toEmail));
        if (string.IsNullOrEmpty(subject)) throw new ArgumentNullException(nameof(subject));
        if (string.IsNullOrEmpty(body)) throw new ArgumentNullException(nameof(body));

        using var smtpClient = new SmtpClient(_options.SmtpServer, _options.Port)
        {
            Credentials = new NetworkCredential(_options.Username, _options.Password),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_options.Username, _options.FromServer),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine($"Message send to {toEmail}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected exception {toEmail}. Error: {ex.Message}");
            throw;
        }
    }
}