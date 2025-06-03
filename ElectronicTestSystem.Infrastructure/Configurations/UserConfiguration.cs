using ElectronicTestSystem.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicTestSystem.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
   

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .IsRequired();

        builder.ComplexProperty(u => u.Name, n =>
        {
            n.Property(x => x.FirstName).IsRequired();
            n.Property(x => x.MiddleName);
            n.Property(x => x.LastName).IsRequired();
        });

        builder.ComplexProperty(u => u.Login, l =>
        {
            l.Property(x => x.Value).IsRequired();
        });

        builder.ComplexProperty(u => u.Password, p =>
        {
            p.Property(x => x.Hash).IsRequired();
        });

        builder.Property(u => u.IsTeacher)
            .IsRequired();

        builder.Property(u => u.IdentityId)
            .IsRequired();

    }
}