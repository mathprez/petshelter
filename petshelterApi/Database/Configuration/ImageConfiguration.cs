using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using petshelterApi.Domain;

namespace petshelterApi.Database
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder
                .HasKey(img => img.Id);
            //builder
            //    .HasAlternateKey(img => new { img.Name, img.ContentType });

            builder
                .Property(img => img.Name)
                .IsRequired()
                .HasMaxLength(50);
            builder
                .Property(img => img.ContentType)
                .IsRequired()
                .HasMaxLength(10);
            builder
               .Property(img => img.Data)
               .IsRequired();
        }
    }
}
