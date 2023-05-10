using messaging_app_API.Models;

namespace messaging_app_API.UtilityServices
{
    public interface IEmailService
    {
        void SendEmail(EmailModel emailModel);
    }
}
