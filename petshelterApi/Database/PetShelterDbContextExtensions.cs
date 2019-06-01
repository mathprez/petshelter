using petshelterApi.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;

namespace petshelterApi.Database
{
    public static class PetShelterDbContextExtensions
    {
        private static Random rnd = new Random();
        public static void Seed(this PetShelterDbContext context)
        {
            context.Pets.RemoveRange(context.Pets);
            context.Breeds.RemoveRange(context.Breeds);
            context.Categories.RemoveRange(context.Categories);
            context.Shelters.RemoveRange(context.Shelters);

            var shelters = new List<Shelter>()
            {
                new Shelter()
                {
                    Name = "Brugge sportwereld BVBA",
                    Address = new Address()
                    {
                        Country  = "Belgium",
                        LineOne = "Legeweg 6",
                        LineTwo = null,
                        LineThree = "8000 Brugge"
                    }
                },
                new Shelter()
                {
                    Name = "Gentse Hondenshelter",
                    Address = new Address()
                    {
                        Country  = "Belgium",
                        LineOne = "Drongensesteenweg 43",
                        LineTwo = null,
                        LineThree = "9000 Gent"
                    }
                },
                new Shelter()
                {
                    Name = "Hasselt Dieren VZW",
                    Address = new Address()
                    {
                        Country  = "Belgium",
                        LineOne = "Moorelweg 96",
                        LineTwo = "Bus A",
                        LineThree = "4505 Morelegem"
                    }
                },
                new Shelter()
                {
                    Name = "Brussel dierenasiel",
                    Address = new Address()
                    {
                        Country  = "Belgium",
                        LineOne = "Heireweg 78",
                        LineTwo = null,
                        LineThree = "2121 Brussel"
                    }
                }
            };

            // data from Aden Forshaw APIs https://thatapiguy.com/
            // https://thedogapi.com
            // https://thecatapi.com
            var cats = GetFromJson(@"Database\SeedFiles\cats.json");
            var dogs = GetFromJson(@"Database\SeedFiles\dogs.json");

            // data from https://github.com/dominictarr/random-name/blob/master/first-names.txt
            var petNames = GetFromFile(@"Database\SeedFiles\petnames.txt");

            context.Pets.AddRange(CreatePets(shelters, cats, petNames, "Cat"));
            context.Pets.AddRange(CreatePets(shelters, dogs, petNames, "Dog"));
            context.SaveChanges();
        }

        private static IEnumerable<Pet> CreatePets(List<Shelter> shelters, Dictionary<string, List<string>> data, string[] petNames, string categoryName)
        {
            var colours = new[] { "Cream", "Dotted grey", "Striped grey", "Dark chocolate", "Brown", "Black", "Orange", "Yellow", "Striped orange" };
            var loremIpsum = "Cras sed nunc id purus scelerisque volutpat sed et ipsum. Nam vitae feugiat orci. Etiam nec sodales odio, a feugiat justo. Nam vitae odio nulla. Etiam vel nulla nec turpis faucibus iaculis et quis enim. Proin non turpis sit amet sapien molestie malesuada. Cras sed vestibulum leo, sed iaculis lorem.";
            var category = new Category() { Name = categoryName };
            foreach (var breedName in data.Keys)
            {
                var breed = new Breed() { Name = breedName, Category = category };
                foreach (var url in data[breedName])
                {
                    yield return new Pet()
                    {
                        Name = petNames[rnd.Next(petNames.Length)],
                        HandlerId = "auth0|5c8954a102d7ad0170b9fa1e",
                        Shelter = shelters[rnd.Next(shelters.Count)],
                        Breed = breed,
                        Gender = rnd.Next(2) == 1 ? Gender.Female : Gender.Male,
                        Color = colours[rnd.Next(colours.Length)],
                        Description = loremIpsum,
                        ExternalImageUrl = url
                    };
                }
            }
        }

        private static Dictionary<string, List<string>> GetFromJson(string fileName)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);
            var cats = new Dictionary<string, List<string>>();
            var serializer = new DataContractJsonSerializer(typeof(Dictionary<string, List<string>>));
            using (var streamReader = new StreamReader(path))
            {
                cats = (Dictionary<string, List<string>>)serializer.ReadObject(streamReader.BaseStream);
            }
            return cats;
        }

        private static string[] GetFromFile(string fileName)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);
            return File.ReadAllLines(path);
        }
    }
}
