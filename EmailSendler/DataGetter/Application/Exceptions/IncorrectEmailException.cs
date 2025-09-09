using Domain.Common;

namespace Application.Exceptions;

public class IncorrectEmailException(string message) : BaseExceptionHandler("Incorrect Email format", 404, message)
{
}