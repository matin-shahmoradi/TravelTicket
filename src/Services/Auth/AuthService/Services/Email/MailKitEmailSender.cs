using AuthService.Interfaces;
using BuildingBlocks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using static System.Net.Mime.MediaTypeNames;

namespace AuthService.Services.Email
{
    public class MailKitEmailSender(ILogger<MailKitEmailSender> logger) : IMailKitEmailSender
    {
        public async Task<Result<string>> SendEmail(string mail, string receiver, CancellationToken cancellationToken)
        {
            var mimeMessage = CreateMimeMessage(mail, receiver);

            var result = await CreateSmtpConnection(mimeMessage, cancellationToken);

            return result;
        }
        private MimeMessage CreateMimeMessage(string mail, string receiver)
        {
            var message = new MimeMessage();

            var from = new MailboxAddress("TravelTicket", "TravelTicket@noreplay.com");

            message.From.Add(from);

            var to = new MailboxAddress("New User", receiver);

            message.To.Add(to);
            message.Subject = "Email verification for TravelTicket";
            message.Body = new TextPart(Text.Html)
            {
                Text = mail
            };

            return message;
        }
        private async Task<Result<string>> CreateSmtpConnection(MimeMessage message, CancellationToken cancellationToken = default)
        {
            using var smtp = new SmtpClient(new MailKit.ProtocolLogger(Console.OpenStandardOutput()));
            smtp.Timeout = 5000;
            try
            {
                await smtp.ConnectAsync("127.0.0.1", 2525, SecureSocketOptions.None);
                var response = await smtp.SendAsync(message, cancellationToken);
                await smtp.DisconnectAsync(true);

                logger.LogInformation("Email successfully sent to {Recipient}. Server response: {Response}", message.To, response);
                return Result<string>.Success(response);
            }
            catch (SmtpCommandException ex)
            {
                logger.LogError(ex, "SMTP command error while sending to {Recipient}. StatusCode: {StatusCode}", message.To, ex.StatusCode);
                return Result<string>.Failure(Error.CustomError(mesaage: $"SMTP exception : {ex.Message}"));
            }

            finally
            {
                if (smtp.IsConnected)
                    await smtp.DisconnectAsync(true);
            }
        }

    }
}
