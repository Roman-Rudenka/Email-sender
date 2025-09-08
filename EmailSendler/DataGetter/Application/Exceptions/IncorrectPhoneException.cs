using Domain.Common;

namespace Application.Exceptions;

public class IncorrectPhoneException(string message) : BaseExceptionHandler("Incorrect Phone number", 404, message)
{
    
}