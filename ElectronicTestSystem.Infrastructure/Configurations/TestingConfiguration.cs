using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicTestSystem.Infrastructure.Configurations;

internal sealed class TestingConfiguration : IEntityTypeConfiguration<Testing>
{
    public void Configure(EntityTypeBuilder<Testing> builder)
    {
        builder.ToTable("testings");

        builder.HasKey(t => t.Id);

        builder.HasOne<Test>(t => t.Test)
            .WithMany()
            .HasForeignKey(t => t.TestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Group>() 
            .WithMany()
            .HasForeignKey(t => t.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>() 
            .WithMany()
            .HasForeignKey(t => t.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.EndedAt)
            .IsRequired();

        builder.Property(t => t.TestId)
            .IsRequired();

        builder.Property(t => t.GroupId)
            .IsRequired();

        builder.Property(t => t.AuthorId)
            .IsRequired();
    }
}