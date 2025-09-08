using Domain.Models;

namespace Application.Interfaces;

public interface IUserDataService
{
    public Task<UserData> GetData(string email, string address, string phoneNumber, CancellationToken cancellationToken);
}