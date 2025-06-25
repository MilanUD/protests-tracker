using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using RoadBlockTracker.Services.Interfaces;
using MailKit.Net.Smtp;

namespace RoadBlockTracker.Services.Implementations
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
        {
            var subject = "Confirm your email";
            var body = $@"<p>Please confirm your email by clicking the link below:</p>
                     <p><a href='{confirmationLink}'>{confirmationLink}</a></p>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            var subject = "Reset your password";
            var body = $@"<p>Please reset your password by clicking the link below:</p>
                     <p><a href='{resetLink}'>{resetLink}</a></p>
                     <p>If you didn't request this, please ignore this email.</p>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var mailSettings = _config.GetSection("MailSettings");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    mailSettings["DisplayName"],
                    mailSettings["From"]));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = subject;

                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = htmlMessage
                };

                using var smtp = new SmtpClient();

                await smtp.ConnectAsync(
                    mailSettings["Host"],
                    mailSettings.GetValue<int>("Port"),
                    SecureSocketOptions.StartTlsWhenAvailable);

                await smtp.AuthenticateAsync(
                    mailSettings["UserName"],
                    mailSettings["Password"]);

                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", email);
                throw;
            }
        }
    }
}
