using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using petshelterApi.Domain;

namespace petshelterApi.Database
{
    public class PetConfiguration : IEntityTypeConfiguration<Pet>
    {
        public void Configure(EntityTypeBuilder<Pet> builder)
        {
            builder
                .HasKey(pet => pet.Id);
            builder
                .HasOne(pet => pet.Breed)
                .WithMany(breed => breed.Pets)
                .HasForeignKey(pet => pet.BreedId)
                .IsRequired();
            builder
                .Property(pet => pet.Name)
                .IsRequired()
                .HasMaxLength(50);
            builder
                .Property(pet => pet.HandlerId)
                .IsRequired()
                .HasMaxLength(50);
            builder
                .Property(pet => pet.ExternalImageUrl)
                .IsRequired(false)
                .HasMaxLength(500);
            builder
                .HasOne(pet => pet.Image)
                .WithOne()
                .HasForeignKey<Pet>(pet => pet.ImageId)
                .IsRequired(false);
            builder
                .Property(pet => pet.Gender)
                .IsRequired();
            builder
                .Property(pet => pet.Description)
                .IsRequired()
                .HasMaxLength(1000);
        }
    }
}
