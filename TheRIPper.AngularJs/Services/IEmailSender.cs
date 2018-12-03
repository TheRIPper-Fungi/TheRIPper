using System.Threading.Tasks;

namespace TheRIPPer.Razor.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}