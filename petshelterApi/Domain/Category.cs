using System.Collections.Generic;
namespace petshelterApi.Domain
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Breed> Breeds { get; set; }
    }
}
