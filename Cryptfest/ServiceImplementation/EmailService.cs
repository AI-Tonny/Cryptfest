using Cryptfest.Enums;
using Cryptfest.Interfaces.Services;
using Cryptfest.Model;
using Cryptfest.Model.Dtos;
using System.Net;
using System.Net.Mail;

namespace Cryptfest.ServiceImplementation;

public class EmailService: IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateVerificationCode()
    {
        return new Random().Next(1000, 9999).ToString();
    }

    public async Task<ToClientDto> SendVerificationEmail(VerificationRequest verificationRequest)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");

        string fromAddress = smtpSettings["Username"]!;
        string password = smtpSettings["Password"]!;
        string host = smtpSettings["Host"]!;
        int port = int.Parse(smtpSettings["Port"]!);
        bool enableSsl = bool.Parse(smtpSettings["EnableSsl"]!);

        string code = GenerateVerificationCode();

        string htmlBody = $@"
            <html>
            <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; text-align: center;'>
                <div style='max-width: 600px; margin: auto; background: white; padding: 30px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0,0,0,0.05);'>
        
                    <h2 style='color: #333;'>Confirm Your Account</h2>
                    <p style='color: #555; font-size: 16px; line-height: 1.5;'>
                        Thank you for registering with us! Please use the following code to complete your verification process.
                    </p>
        
                    <div style='margin: 30px 0; padding: 15px; background-color: #f0f0ff; border: 1px solid #c0c0ff; border-radius: 5px; display: inline-block;'>
                        <strong style='font-size: 28px; letter-spacing: 5px; color: #4a4aff;'>{code}</strong>
                    </div>
        
                    <p style='color: #777; font-size: 14px;'>
                        This code is valid for 5 minutes. If you did not request this, please ignore this email.
                    </p>

                    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                    <p style='font-size: 12px; color: #aaa;'>
                        &copy; {DateTime.Now.Year} Cryptfest. All rights reserved.
                    </p>
        
                </div>
            </body>
            </html>";

        var mail = new MailMessage(fromAddress, verificationRequest.requestEmail)
        {
            Subject = "Account Verification Code",
            Body = htmlBody,
            IsBodyHtml = true
        };

        using (var smtpClient = new SmtpClient(host, port))
        {
            smtpClient.Credentials = new NetworkCredential(fromAddress, password);
            smtpClient.EnableSsl = enableSsl;

            await smtpClient.SendMailAsync(mail);
        }

        return new ToClientDto()
        {
            Status = ResponseStatus.Success,
            Data = code
        };
    }
}
