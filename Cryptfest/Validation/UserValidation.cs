using Cryptfest.Interfaces.Validation;
using Cryptfest.Model;

namespace Cryptfest.Validation;

public class UserValidation : IUserValidation
{
    public ValidationResult IsLoginValid(string login)
    {
        bool isLoginValid = string.IsNullOrEmpty(login) || login.Length < 3;
        return new ValidationResult()
        {
            isValid = !isLoginValid,
            Message = isLoginValid ? "Login cannot be empty or less than 3 characters" : ""
        };
    }

    public ValidationResult IsPasswordValid(string password)
    {
        bool isPasswordValid = string.IsNullOrEmpty(password) || password.Length < 6;
        return new ValidationResult()
        {
            isValid = !isPasswordValid,
            Message = isPasswordValid ? "Password cannot be empty or less than 6 characters" : ""
        };
    }
}
