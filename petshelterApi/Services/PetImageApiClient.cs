using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
namespace petshelterApi.Services
{
    // data from Aden Forshaw APIs https://thatapiguy.com/
    // https://thedogapi.com
    // https://thecatapi.com

    public class DogImageApiClient : PetImageApiClient
    {
        public DogImageApiClient(HttpClient client, IConfiguration config)
        {
            client.BaseAddress = new Uri(config["PetApi:Dog:Audience"]);
            client.DefaultRequestHeaders.Add("x-api-key", config["PetApi:Dog:Token"]);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client = client;
        }
    }

    public class CatImageApiClient : PetImageApiClient
    {
        public CatImageApiClient(HttpClient client, IConfiguration config)
        {
            client.BaseAddress = new Uri(config["PetApi:Cat:Audience"]);
            client.DefaultRequestHeaders.Add("x-api-key", config["PetApi:Cat:Token"]);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client = client;
        }
    }

    public abstract class PetImageApiClient
    {
        protected HttpClient _client;
        public async Task WriteTestData(string path)
        {
            var urlsTask = GetUrlsByBreed();
            var serializer = new DataContractJsonSerializer(typeof(Dictionary<string, List<string>>));
            using (var file = File.Create(path))
            {
                serializer.WriteObject(file, await urlsTask);
            }
        }

        private async Task<Dictionary<string, List<string>>> GetUrlsByBreed()
        {
            var breeds = await GetBreedDictionary();
            var serializer = new DataContractJsonSerializer(typeof(List<picture>));
            var images = new Dictionary<string, List<string>>();
            foreach (var key in breeds.Keys)
            {
                var streamTask = _client.GetStreamAsync($"images/search?breed_id={key}&limit=3");
                var imagesByBreed = ((List<picture>)serializer.ReadObject(await streamTask)).Select(x => x.url).ToList();
                if (imagesByBreed.Any()) images.Add(breeds[key], imagesByBreed);
            }
            return images;
        }

        private async Task<Dictionary<string, string>> GetBreedDictionary()
        {
            var serializer = new DataContractJsonSerializer(typeof(List<breed>));
            var streamTask = _client.GetStreamAsync("breeds");
            return ((List<breed>)serializer.ReadObject(await streamTask)).ToDictionary(x => x.id, x => x.name);
        }

        [DataContract]
        internal class breed
        {
            [DataMember]
            public string id { get; set; }
            [DataMember]
            public string name { get; set; }
        }

        [DataContract]
        internal class picture
        {
            [DataMember]
            public string url { get; set; }
        }
    }
}
