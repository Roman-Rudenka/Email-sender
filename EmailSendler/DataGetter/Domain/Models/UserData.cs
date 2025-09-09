namespace Domain.Models;

public class UserData(string email, string address, string phoneNumber)
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string Email { get; init; } = email;
    public string  Address { get; init; } = address;
    public string PhoneNumber { get; init; } = phoneNumber;    
}