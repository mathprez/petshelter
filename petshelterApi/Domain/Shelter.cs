using System.Collections.Generic;

namespace petshelterApi.Domain
{
    public class Shelter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public List<Pet> Pets { get; set; }
    }
}
