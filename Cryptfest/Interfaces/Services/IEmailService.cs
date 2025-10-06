namespace Cryptfest.Interfaces.Services;

public interface IEmailService
{
    string GenerateVerificationCode();
    Task<string> SendVerificationEmail(string recipientEmail);
}
