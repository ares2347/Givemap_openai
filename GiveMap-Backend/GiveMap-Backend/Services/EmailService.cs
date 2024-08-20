using System.Net;
using System.Net.Mail;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace GiveMap_Backend.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string email, string token);
    Task SendDonationStatusUpdateEmailAsync(string toEmail, string donationStatus);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(string email, string token)
    {
        var resetUrl = $"{_configuration["AppUrl"]}/reset-password?token={token}&email={email}";
        var subject = "Password Reset Request";
        var body = $"Your donation status has been updated to: {resetUrl}";
        await SendEmailAsync(email, subject, body);

        _logger.LogInformation($"Password reset email sent to: {email}");
    }
    
    public async Task SendDonationStatusUpdateEmailAsync(string toEmail, string donationStatus)
    {
        var subject = "Donation Status Update";
        var body = $"Your donation status has been updated to: {donationStatus}";
        await SendEmailAsync(toEmail, subject, body);
        
        _logger.LogInformation($"Donation status update email sent to: {toEmail}");
    }
    
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient();
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("GiveMap Support", _configuration["Email:From"]));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;
        message.Body = new TextPart("plain")
        {
            Text = body
        };

        try
        {
            await client.ConnectAsync(_configuration["Email:SmtpServer"], 587, false);
            await client.AuthenticateAsync(_configuration["Email:Username"], _configuration["Email:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            _logger.LogInformation($"Email sent successfully to {toEmail}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending email to {toEmail}");
            throw;
        }
    }
    

    
}