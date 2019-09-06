using MFS.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MFS.Data
{
    internal class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.HasIndex(c => c.ContactNumber).IsUnique();
            builder.HasIndex(c => c.EMailAddress).IsUnique();
            builder.Property(c => c.EMailAddress).IsRequired().HasMaxLength(100);
            builder.Property(c => c.FirstName).IsRequired().HasMaxLength(25);
            builder.Property(c => c.LastName).IsRequired().HasMaxLength(25);
            builder.Property(c => c.ContactNumber).HasMaxLength(20);
        }
    }
}
