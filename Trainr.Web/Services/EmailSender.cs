using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MailKit.Net.Smtp;

namespace Trainr.Web.Services
{
    public class EmailSender : IEmailSender
    {
        // TODO: Move credentials to User Secrets or environment configuration before deployment.
        // These values must never be committed to source control in a production project.
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                string fromAddress      = "Test.Kam.Hayer@gmail.com";
                string fromAddressTitle = "WV HS Soccer App Admin";
                string toAddressTitle   = "WV Soccer App User";
                string smtpServer       = "smtp.gmail.com";
                int    smtpPort         = 587;

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(fromAddressTitle, fromAddress));
                mimeMessage.To.Add(new MailboxAddress(toAddressTitle, email));
                mimeMessage.Subject = subject;
                mimeMessage.Body    = new TextPart("html") { Text = message };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpServer, smtpPort, false);
                    await client.AuthenticateAsync("Test.Kam.Hayer@gmail.com", "Pudduu88!");
                    await client.SendAsync(mimeMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
