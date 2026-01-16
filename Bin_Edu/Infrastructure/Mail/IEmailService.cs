namespace Student_Science_Research_Management_UEF.Infrastructure.Mail
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlMessage);
    }
}
