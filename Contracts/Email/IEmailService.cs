using GrowRoomEnvironment.Contracts.Services;
using MimeKit;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.Contracts.Email
{
    public interface IEmailService : IService
    {
        Task<(bool success, string errorMsg)> SendEmailAsync(MailboxAddress sender, MailboxAddress[] recepients, string subject, string body, ISmtpConfig config = null, bool isHtml = true);
        Task<(bool success, string errorMsg)> SendEmailAsync(string recepientName, string recepientEmail, string subject, string body, ISmtpConfig config = null, bool isHtml = true);
        Task<(bool success, string errorMsg)> SendEmailAsync(string senderName, string senderEmail, string recepientName, string recepientEmail, string subject, string body, ISmtpConfig config = null, bool isHtml = true);
    }
}
