using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserDataRepository(AppDbContext context) : IUserDataRepository
{
    public async Task AddUserDataAsync(UserData userData, CancellationToken cancellationToken) 
    { 
        await context.UserDatas.AddAsync(userData, cancellationToken);
    }
    
    public Task SaveChangesAsync(CancellationToken cancellationToken) 
    { 
        return context.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserData?> GetDataByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await context.UserDatas.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
        
}