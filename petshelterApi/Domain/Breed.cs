using System.Collections.Generic;

namespace petshelterApi.Domain
{
    public class Breed
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<Pet> Pets { get; set; }
    }
}