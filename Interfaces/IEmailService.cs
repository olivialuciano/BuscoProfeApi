namespace BuscoProfe.Api.Interfaces;

public interface IEmailService
{
    Task SendEmailVerificationCodeAsync(string toEmail, string code);
}