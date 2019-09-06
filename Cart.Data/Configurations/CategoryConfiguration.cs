using MFS.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MFS.Data
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasIndex(t => t.CategoryName).IsUnique();

            builder.Property(t => t.CategoryName).IsRequired().HasMaxLength(60);

        }
    }
}
