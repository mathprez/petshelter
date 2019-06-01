using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using petshelterApi.Domain;

namespace petshelterApi.Database
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder
                .HasKey(cat => cat.Id);
            builder
                .HasAlternateKey(cat => cat.Name);

            builder
                .Property(cat => cat.Name)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
