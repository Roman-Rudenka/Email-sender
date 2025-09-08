namespace Domain.Common;

public class BaseExceptionHandler(string status, int statusCode, string message) : Exception(message)
{
    public string Status { get; set; } = status;
    public int StatusCode { get; set; } = statusCode;
}