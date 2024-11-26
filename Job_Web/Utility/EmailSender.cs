using Microsoft.AspNetCore.Identity.UI.Services;

namespace Job_Web.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //Implementation email services
            return Task.CompletedTask;
        }
    }
}
