using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using petshelterApi.Domain;

namespace petshelterApi.Database
{
    public class BreedConfiguration : IEntityTypeConfiguration<Breed>
    {
        public void Configure(EntityTypeBuilder<Breed> builder)
        {
            builder
                .HasKey(breed => breed.Id);
            builder
                .HasAlternateKey(breed => new { breed.Name, breed.CategoryId });
            builder
                .HasOne(breed => breed.Category)
                .WithMany(cat => cat.Breeds)
                .HasForeignKey(breed => breed.CategoryId);

            builder
                .Property(breed => breed.Name)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
