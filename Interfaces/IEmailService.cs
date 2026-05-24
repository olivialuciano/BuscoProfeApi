namespace BuscoProfe.Api.Interfaces;

public interface IEmailService
{
    Task SendEmailVerificationCodeAsync(string toEmail, string code);

    Task SendPasswordResetCodeAsync(string toEmail, string code);
}