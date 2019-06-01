using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using petshelterApi.Domain;

namespace petshelterApi.Database
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder
                .HasKey(a => a.Id);
            builder
                .HasOne(a => a.Pet)
                .WithMany()
                .HasForeignKey(a => a.PetId);

            builder
                .Property(a => a.UserId)
                .IsRequired()
                .HasMaxLength(50);
            builder
                .Property(a => a.Start)
                .IsRequired();
        }
    }
}
