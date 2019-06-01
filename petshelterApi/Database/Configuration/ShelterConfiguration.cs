using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using petshelterApi.Domain;

namespace petshelterApi.Database
{
    public class ShelterConfiguration : IEntityTypeConfiguration<Shelter>
    {
        public void Configure(EntityTypeBuilder<Shelter> builder)
        {
            builder
                .HasKey(shelter => shelter.Id);
            builder
                .HasAlternateKey(shelter => shelter.Name);
            builder
                .HasMany(shelter => shelter.Pets)
                .WithOne(pet => pet.Shelter)
                .HasForeignKey(pet => pet.ShelterId);

            builder
                .Property(shelter => shelter.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.OwnsOne(shelter => shelter.Address, sa =>
            {
                sa.Property(a => a.Country)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Country");
                sa.Property(a => a.LineOne)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasColumnName("AddressLineOne");
                sa.Property(a => a.LineTwo)
                   .IsRequired(false)
                   .HasMaxLength(50)
                   .HasColumnName("AddressLineTwo");
                sa.Property(a => a.LineThree)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasColumnName("AddressLineThree");
            });
        }
    }
}
