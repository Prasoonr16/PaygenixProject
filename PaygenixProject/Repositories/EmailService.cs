using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;



namespace PaygenixProject.Repositories
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Host = smtpSettings["Host"];
                    smtpClient.Port = int.Parse(smtpSettings["Port"]);
                    smtpClient.EnableSsl = bool.Parse(smtpSettings["EnableSsl"]);
                    smtpClient.Credentials = new NetworkCredential(
                        smtpSettings["Username"],
                        smtpSettings["Password"]
                    );

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpSettings["Username"], "Admin"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false // Plaintext email
                    };
                    mailMessage.To.Add(toEmail);

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (SmtpException ex)
            {
                throw new Exception($"SMTP Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Email sending failed: {ex.Message}");
            }
        }
    }

}
