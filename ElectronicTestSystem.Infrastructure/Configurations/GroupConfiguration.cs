using ElectronicTestSystem.Domain.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicTestSystem.Infrastructure.Configurations;

internal sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("groups");

        builder.HasKey(g => g.Id);

        builder.ComplexProperty(g => g.Name, n =>
        {
            n.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(100);
        });
        
        builder.HasMany(g => g.Users)
            .WithMany(u => u.Groups)
            .UsingEntity("group_user");
    }
}