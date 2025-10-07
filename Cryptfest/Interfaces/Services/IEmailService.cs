using Cryptfest.Model;
using Cryptfest.Model.Dtos;

namespace Cryptfest.Interfaces.Services;

public interface IEmailService
{
    string GenerateVerificationCode();
    Task<ToClientDto> SendVerificationEmail(VerificationRequest verificationRequest);
}
