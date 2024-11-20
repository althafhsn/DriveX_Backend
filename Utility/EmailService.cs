using DriveX_Backend.Entities.Users.Models;
using MimeKit;
using MailKit.Net.Smtp;

namespace DriveX_Backend.Utility
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendPasswordResetEmail(EmailModel emailModel)
        {
            var emailMessage = new MimeMessage();

            // Set the sender email address and display name
            var from = _configuration["EmailSettings:From"];
            emailMessage.From.Add(new MailboxAddress("DriveX", from));

            // Log the recipient email
   

            // Set the recipient email address
            emailMessage.To.Add(new MailboxAddress(emailModel.To, emailModel.To));

            // Set the email subject and body
            emailMessage.Subject = emailModel.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailModel.Body
            };

            using var client = new SmtpClient();
            try
            {
                // Connect to the SMTP server
                client.Connect(
                    _configuration["EmailSettings:SmtpServer"],
                    int.Parse(_configuration["EmailSettings:Port"]),
                    true // Use SSL
                );

                // Authenticate using the credentials
                client.Authenticate(
                    _configuration["EmailSettings:Username"],
                    _configuration["EmailSettings:Password"]
                );

                // Send the email
                client.Send(emailMessage);
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                // Disconnect and dispose of the SMTP client
                client.Disconnect(true);
                client.Dispose();
            }
        }


    }
}
