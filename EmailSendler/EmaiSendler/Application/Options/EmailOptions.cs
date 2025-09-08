namespace Application.Options;

public class EmailOptions
{
    public required string SmtpServer { get; init; }
    public required int Port  { get; init; }
    public required string Username  { get; init; }
    public required string Password { get; init; }
    public required string FromServer { get; init; }
}