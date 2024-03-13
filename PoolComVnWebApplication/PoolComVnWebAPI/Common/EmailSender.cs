using System.Net.Mail;
using System.Net;

namespace PoolComVnWebAPI.Common
{
    public class EmailSender : IEmailSender
    {
        public Task SendMailAsync(string email, string username, string verifyCode)
        {
            var mail = "poolcomvn@gmail.com";
            var pw = "vyfi yuwu qwrx znhm";
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw),
            };

            var message = new MailMessage(from: mail, to: email);
            message.Subject = "Verify Code - PoolComVN";
            message.Body = CreateVerifyEmail(username, verifyCode);
            message.IsBodyHtml = true;
            return client.SendMailAsync(message);
        }

        private string CreateVerifyEmail(string username, string verifyCode)
        {
            // Lấy đường dẫn thư mục chứa controller (Template folder)
            string controllerDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(
                Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory))));

            // Xây dựng đường dẫn tương đối đến file trong thư mục Template
            string relativePath = Path.Combine("EmailTemplate", "RegisterConfirm.html");
            string filePath = Path.Combine(controllerDirectory, relativePath);
            try
            {
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{username}", username);
                body = body.Replace("{verifyCode}", verifyCode);
                return body;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
