using DriveX_Backend.Entities.Users.Models;

namespace DriveX_Backend.Utility
{
    public interface IEmailService
    {
        void SendPasswordResetEmail(EmailModel emailModel);
    }
}
