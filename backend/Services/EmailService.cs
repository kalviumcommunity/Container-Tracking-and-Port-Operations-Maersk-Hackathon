using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Backend.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Backend.Services
{
    public interface IEmailService
    {
        Task SendEventNotificationAsync(EventDto eventDto);
        Task SendCriticalAlertAsync(EventDto eventDto);
    }

    public class EmailSettings
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string SenderPassword { get; set; } = string.Empty;
        public List<string> NotificationRecipients { get; set; } = new List<string>();
    }

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSettings _settings;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _logger = logger;
            
            // Read from environment variables first, fallback to appsettings.json
            _settings = new EmailSettings
            {
                SmtpHost = Environment.GetEnvironmentVariable("EMAIL_SMTP_HOST") 
                    ?? configuration["Email:SmtpHost"] 
                    ?? "smtp.gmail.com",
                SmtpPort = int.TryParse(Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT"), out var port) 
                    ? port 
                    : configuration.GetValue<int>("Email:SmtpPort", 587),
                EnableSsl = bool.TryParse(Environment.GetEnvironmentVariable("EMAIL_ENABLE_SSL"), out var ssl) 
                    ? ssl 
                    : configuration.GetValue<bool>("Email:EnableSsl", true),
                SenderEmail = Environment.GetEnvironmentVariable("EMAIL_SENDER_EMAIL") 
                    ?? configuration["Email:SenderEmail"] 
                    ?? string.Empty,
                SenderName = Environment.GetEnvironmentVariable("EMAIL_SENDER_NAME") 
                    ?? configuration["Email:SenderName"] 
                    ?? "Maersk Port Operations",
                SenderPassword = Environment.GetEnvironmentVariable("EMAIL_SENDER_PASSWORD") 
                    ?? configuration["Email:SenderPassword"] 
                    ?? string.Empty,
                NotificationRecipients = ParseRecipients(
                    Environment.GetEnvironmentVariable("EMAIL_NOTIFICATION_RECIPIENTS"),
                    configuration.GetSection("Email:NotificationRecipients").Get<List<string>>()
                )
            };
            
            _logger.LogInformation("Email service configured with {RecipientCount} recipients", 
                _settings.NotificationRecipients.Count);
        }
        
        private List<string> ParseRecipients(string? envRecipients, List<string>? configRecipients)
        {
            if (!string.IsNullOrEmpty(envRecipients))
            {
                return new List<string>(envRecipients.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim()));
            }
            return configRecipients ?? new List<string>();
        }

        public async Task SendEventNotificationAsync(EventDto eventDto)
        {
            try
            {
                // Only send emails for High or Critical severity
                if (eventDto.Severity != "High" && eventDto.Severity != "Critical")
                {
                    return;
                }

                var subject = $"🚨 {eventDto.Severity} Event: {eventDto.EventType}";
                var body = BuildEmailBody(eventDto);

                await SendEmailAsync(_settings.NotificationRecipients ?? new List<string>(), subject, body);

                _logger.LogInformation("Email notification sent for event {EventId}", eventDto.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email notification for event {EventId}", eventDto.EventId);
            }
        }

        public async Task SendCriticalAlertAsync(EventDto eventDto)
        {
            try
            {
                var subject = $"⚠️ CRITICAL ALERT: {eventDto.EventType}";
                var body = BuildCriticalAlertBody(eventDto);

                await SendEmailAsync(_settings.NotificationRecipients ?? new List<string>(), subject, body, isHighPriority: true);

                _logger.LogInformation("Critical alert email sent for event {EventId}", eventDto.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send critical alert email for event {EventId}", eventDto.EventId);
            }
        }

        private async Task SendEmailAsync(List<string> recipients, string subject, string body, bool isHighPriority = false)
        {
            if (string.IsNullOrEmpty(_settings.SenderEmail) || string.IsNullOrEmpty(_settings.SenderPassword))
            {
                _logger.LogWarning("Email settings not configured. Skipping email send.");
                return;
            }

            if (recipients == null || recipients.Count == 0)
            {
                _logger.LogWarning("No email recipients configured. Skipping email send.");
                return;
            }

            using var smtpClient = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.SenderPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Timeout = 30000
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            if (isHighPriority)
            {
                mailMessage.Priority = MailPriority.High;
            }

            foreach (var recipient in recipients)
            {
                if (!string.IsNullOrWhiteSpace(recipient))
                {
                    mailMessage.To.Add(recipient);
                }
            }

            if (mailMessage.To.Count == 0)
            {
                _logger.LogWarning("No valid email recipients found. Skipping email send.");
                return;
            }

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {RecipientCount} recipients", mailMessage.To.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMTP error while sending email. Host: {Host}, Port: {Port}", 
                    _settings.SmtpHost, _settings.SmtpPort);
                throw;
            }
        }

        private string BuildEmailBody(EventDto eventDto)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #1e40af; color: white; padding: 20px; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f8fafc; padding: 20px; border: 1px solid #e2e8f0; }}
        .severity-critical {{ color: #dc2626; font-weight: bold; }}
        .severity-high {{ color: #ea580c; font-weight: bold; }}
        .severity-medium {{ color: #ca8a04; font-weight: bold; }}
        .info-row {{ margin: 10px 0; }}
        .label {{ font-weight: bold; color: #475569; }}
        .footer {{ margin-top: 20px; padding: 15px; background-color: #e2e8f0; font-size: 12px; text-align: center; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>🚢 Maersk Port Operations Alert</h2>
        </div>
        <div class='content'>
            <h3>{eventDto.EventType}</h3>
            <div class='info-row'>
                <span class='label'>Severity:</span> 
                <span class='severity-{eventDto.Severity?.ToLower()}'>{eventDto.Severity}</span>
            </div>
            <div class='info-row'>
                <span class='label'>Title:</span> {eventDto.Title}
            </div>
            <div class='info-row'>
                <span class='label'>Description:</span> {eventDto.Description}
            </div>
            <div class='info-row'>
                <span class='label'>Time:</span> {eventDto.EventTime:yyyy-MM-dd HH:mm:ss}
            </div>
            <div class='info-row'>
                <span class='label'>Source:</span> {eventDto.Source}
            </div>
            {(eventDto.ContainerId != null ? $"<div class='info-row'><span class='label'>Container ID:</span> {eventDto.ContainerId}</div>" : "")}
            {(eventDto.ShipId != null ? $"<div class='info-row'><span class='label'>Ship:</span> {eventDto.ShipName ?? eventDto.ShipId.ToString()}</div>" : "")}
            {(eventDto.PortId != null ? $"<div class='info-row'><span class='label'>Port:</span> {eventDto.PortName ?? eventDto.PortId.ToString()}</div>" : "")}
            {(eventDto.BerthId != null ? $"<div class='info-row'><span class='label'>Berth:</span> {eventDto.BerthName ?? eventDto.BerthId.ToString()}</div>" : "")}
        </div>
        <div class='footer'>
            This is an automated notification from Maersk Port Operations System.<br>
            Event ID: {eventDto.EventId} | Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC
        </div>
    </div>
</body>
</html>";
        }

        private string BuildCriticalAlertBody(EventDto eventDto)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #dc2626; color: white; padding: 20px; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #fef2f2; padding: 20px; border: 2px solid #dc2626; }}
        .alert-box {{ background-color: #fee2e2; padding: 15px; border-left: 4px solid #dc2626; margin: 15px 0; }}
        .info-row {{ margin: 10px 0; }}
        .label {{ font-weight: bold; color: #7f1d1d; }}
        .footer {{ margin-top: 20px; padding: 15px; background-color: #fee2e2; font-size: 12px; text-align: center; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>🚨🚨 CRITICAL ALERT 🚨🚨</h2>
            <h3>Immediate Action Required</h3>
        </div>
        <div class='content'>
            <div class='alert-box'>
                <h3>⚠️ {eventDto.EventType}</h3>
                <p><strong>{eventDto.Title}</strong></p>
                <p>{eventDto.Description}</p>
            </div>
            <div class='info-row'>
                <span class='label'>Event Time:</span> {eventDto.EventTime:yyyy-MM-dd HH:mm:ss}
            </div>
            <div class='info-row'>
                <span class='label'>Event ID:</span> {eventDto.EventId}
            </div>
            <div class='info-row'>
                <span class='label'>Source:</span> {eventDto.Source}
            </div>
            {(eventDto.ContainerId != null ? $"<div class='info-row'><span class='label'>Container ID:</span> {eventDto.ContainerId}</div>" : "")}
            {(eventDto.ShipId != null ? $"<div class='info-row'><span class='label'>Ship:</span> {eventDto.ShipName ?? eventDto.ShipId.ToString()}</div>" : "")}
            {(eventDto.PortId != null ? $"<div class='info-row'><span class='label'>Port:</span> {eventDto.PortName ?? eventDto.PortId.ToString()}</div>" : "")}
            {(eventDto.BerthId != null ? $"<div class='info-row'><span class='label'>Berth:</span> {eventDto.BerthName ?? eventDto.BerthId.ToString()}</div>" : "")}
            
            <div class='alert-box' style='margin-top: 20px;'>
                <h4>📞 Recommended Actions:</h4>
                <ul>
                    <li>Review event details immediately</li>
                    <li>Contact relevant stakeholders</li>
                    <li>Assess impact on operations</li>
                    <li>Update event status in the system</li>
                </ul>
            </div>
        </div>
        <div class='footer'>
            <strong>CRITICAL ALERT</strong> - This requires immediate attention<br>
            Maersk Port Operations System | {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC
        </div>
    </div>
</body>
</html>";
        }
    }
}
