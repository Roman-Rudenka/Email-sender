using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class UserDataConfiguration: IEntityTypeConfiguration<UserData>
{
    public void Configure(EntityTypeBuilder<UserData> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Address).IsRequired().HasMaxLength(250);
        builder.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.ToTable("UserData");
    }
}