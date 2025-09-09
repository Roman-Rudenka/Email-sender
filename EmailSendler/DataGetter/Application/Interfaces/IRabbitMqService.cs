namespace Application.Interfaces;

public interface IRabbitMqService
{
    public void SendEmail(string email);
}