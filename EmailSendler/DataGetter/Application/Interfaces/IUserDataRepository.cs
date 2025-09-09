using Domain.Models;

namespace Application.Interfaces;

public interface IUserDataRepository
{
    public Task AddUserDataAsync(UserData userData, CancellationToken cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken);
    public Task<UserData?> GetDataByEmailAsync(string email, CancellationToken cancellationToken);
}