namespace petshelterApi.Features.Models
{
    public class ReadPet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Shelter { get; set; }
        public string Breed { get; set; }
        public int Gender { get; set; }
        public string Base64Image { get; set; }
        public string ExternalImage { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }
}
