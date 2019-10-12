using MSF.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSF.Data
{
    internal class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.HasKey(p => p.TenantId);
            builder.HasIndex(p => p.TenantName).IsUnique();
            builder.Property(p => p.TenantName).IsRequired().HasMaxLength(40);
            builder.Property(p => p.DataBaseName).IsRequired().HasMaxLength(60);
        }
    }
}
