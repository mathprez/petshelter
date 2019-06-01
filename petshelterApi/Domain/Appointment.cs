using System;
namespace petshelterApi.Domain
{
    public class Appointment
    {
        public int Id { get; set; }
        public Pet Pet { get; set; }
        public int PetId { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset Start { get; set; }
    }
}
