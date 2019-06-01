using System.ComponentModel.DataAnnotations.Schema;

namespace petshelterApi.Domain
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HandlerId { get; set; }
        public int ShelterId { get; set; }
        public Shelter Shelter { get; set; }
        public int BreedId { get; set; }
        public Breed Breed { get; set; }
        public Gender Gender { get; set; }
        public string Color { get; set; }
        public string ExternalImageUrl { get; set; }
        public int? ImageId { get; set; }
        public Image Image { get; set; }    
        public string Description { get; set; }
    }
}
