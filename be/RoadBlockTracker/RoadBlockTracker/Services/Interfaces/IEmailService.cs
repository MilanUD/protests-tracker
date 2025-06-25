namespace RoadBlockTracker.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(string email, string confirmationLink);
        Task SendPasswordResetEmailAsync(string email, string resetLink);
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
