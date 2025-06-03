using ElectronicTestSystem.Domain.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicTestSystem.Infrastructure.Configurations;

internal sealed class TestConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> builder)
    {
        builder.ToTable("tests");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Id)
            .IsRequired();
            
        builder.Property(t => t.CreatedAt)
            .IsRequired();
            
        builder.Property(t => t.AuthorId)
            .IsRequired();
            
        builder.ComplexProperty(t => t.Subject, s =>
        {
            s.Property(x => x.Name)
                .IsRequired();
        });
        
        builder.ComplexProperty(t => t.Theme, t =>
        {
            t.Property(x => x.Name)
                .IsRequired();
        });
        
        builder.HasMany(t => t.Questions)
            .WithOne()
            .HasForeignKey(q => q.TestId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Ignore(t => t.CorrectQuestionsAnswers);
        builder.Ignore(t => t.CorrectQuestionsAnswersOrderedById);
        builder.Ignore(t => t.Name);
    }
}