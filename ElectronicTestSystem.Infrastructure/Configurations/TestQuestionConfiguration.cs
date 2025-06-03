using ElectronicTestSystem.Domain.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicTestSystem.Infrastructure.Configurations;

internal sealed class TestQuestionConfiguration : IEntityTypeConfiguration<TestQuestion>
{
    public void Configure(EntityTypeBuilder<TestQuestion> builder)
    {
        builder.ToTable("test_questions");

        builder.HasKey(q => q.Id);

        builder.HasOne<Test>()
            .WithMany(t => t.Questions)
            .HasForeignKey(q => q.TestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ComplexProperty(q => q.Body, b =>
        {
            b.Property(x => x.Text).IsRequired();
        });

        builder.Property(q => q.Answers)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.ComplexProperty(q => q.TrueAnswerNumber, n =>
        {
            n.Property(x => x.Value).IsRequired();
        });
    }
}