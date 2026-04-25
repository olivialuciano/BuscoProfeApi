using BuscoProfe.Api.Interfaces;
using BuscoProfe.Api.Options;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BuscoProfe.Api.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailVerificationCodeAsync(string toEmail, string code)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(
            _emailSettings.FromName,
            _emailSettings.FromEmail
        ));

        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Tu código de verificación - Busco Profe";

        message.Body = new BodyBuilder
        {
            HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; color: #111827;'>
                    <h2>Verificá tu cuenta en Busco Profe</h2>

                    <p>Gracias por registrarte.</p>

                    <p>Tu código de verificación es:</p>

                    <div style='
                        font-size: 32px;
                        font-weight: bold;
                        letter-spacing: 8px;
                        background: #f3f4f6;
                        padding: 16px 24px;
                        border-radius: 12px;
                        display: inline-block;
                        margin: 12px 0;
                    '>
                        {code}
                    </div>

                    <p>Este código vence en 10 minutos.</p>

                    <p>Si no creaste una cuenta en Busco Profe, ignorá este email.</p>
                </div>
            ",
            TextBody = $"Tu código de verificación de Busco Profe es: {code}. Vence en 10 minutos."
        }.ToMessageBody();

        using var client = new MailKit.Net.Smtp.SmtpClient();

        await client.ConnectAsync(
            _emailSettings.SmtpHost,
            _emailSettings.SmtpPort,
            SecureSocketOptions.StartTls
        );

        await client.AuthenticateAsync(
            _emailSettings.SmtpUser,
            _emailSettings.SmtpPassword
        );

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}