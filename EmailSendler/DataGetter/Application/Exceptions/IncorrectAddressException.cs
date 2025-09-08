using Domain.Common;

namespace Application.Exceptions;

public class IncorrectAddressException(string message) : BaseExceptionHandler("Incorrect address", 404, message)
{
}