using System.Text.Json;
using System.Text.Json.Serialization;
using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Domain.Testings.Calculators;
using ElectronicTestSystem.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicTestSystem.Infrastructure.Configurations;

internal sealed class TestingUserAnswersConfiguration : IEntityTypeConfiguration<TestingUserAnswers>
{
    public void Configure(EntityTypeBuilder<TestingUserAnswers> builder)
    {
        builder.ToTable("testing_user_answers");

        builder.HasKey(x => x.Id);

        builder.HasOne<Testing>()
            .WithMany()
            .HasForeignKey(x => x.TestingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.TestingId)
            .IsRequired();

        builder.Property(x => x.Answers)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = false
                }),
                v => JsonSerializer.Deserialize<UserAnswer[]>(v, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? Array.Empty<UserAnswer>());

        builder.OwnsOne(x => x.Mark, m =>
        {
            m.Property(m => m.Value);
        });

        builder.Property<uint>("Version").IsRowVersion();
        
        builder.Ignore(x => x.UserName);
        builder.Ignore(x => x.AnswersNumbers);
    }
}