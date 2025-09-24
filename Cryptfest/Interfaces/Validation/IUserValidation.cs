using Cryptfest.Model;

namespace Cryptfest.Interfaces.Validation;

public interface IUserValidation
{
    ValidationResult IsLoginValid(string login);
    ValidationResult IsPasswordValid(string password);
}
