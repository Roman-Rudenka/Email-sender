namespace Application.Interfaces;

public interface IQueryService
{ 
    public Task SendEmailMessageAsync(string toEmail, string subject, string body);
}