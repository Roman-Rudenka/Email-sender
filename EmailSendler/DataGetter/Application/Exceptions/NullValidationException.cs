using Domain.Common;

namespace Application.Exceptions;

public class NullValidationException(string message) : BaseExceptionHandler("Null validation error", 400, message)
{
}