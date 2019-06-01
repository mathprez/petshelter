using petshelterApi.Domain.User;
using petshelterApi.Helpers;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace petshelterApi.Services
{
    public class Auth0ApiClient : IAuth0ApiClient
    {
        private readonly HttpClient _client;
        private readonly DataContractJsonSerializerSettings _serializerSettings;
        
        //access token?
        public Auth0ApiClient(HttpClient httpClient)
        {
            _client = httpClient;
            _serializerSettings = new DataContractJsonSerializerSettings() { EmitTypeInformation = EmitTypeInformation.Never };
        }

        public async Task<Auth0UserList> GetUsers(UserParameters userParameters)
        {
            var serializer = new DataContractJsonSerializer(typeof(Auth0UserList), _serializerSettings);
            var getUsersTask = _client.GetStreamAsync("users" + userParameters.QueryString);
            return (Auth0UserList)serializer.ReadObject(await getUsersTask);
        }

        public async Task<Auth0ReadUser> GetUser(string id)
        {
            var serializer = new DataContractJsonSerializer(typeof(Auth0ReadUser), _serializerSettings);
            var getUserTask = _client.GetStreamAsync($"users/{id}");
            return (Auth0ReadUser)serializer.ReadObject(await getUserTask);
        }

        public async Task<HttpResponseMessage> UpdateUser(Auth0UpdateUser user)
        {
            Task<HttpResponseMessage> result;
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            {
                var serializer = new DataContractJsonSerializer(typeof(Auth0UpdateUser), _serializerSettings);
                serializer.WriteObject(stream, user);
                stream.Position = 0;
                result = _client.PatchAsync(
                    $"users/{user.Id}",
                    new StringContent(await reader.ReadToEndAsync(), Encoding.UTF8, "application/json"));
            }
            return await result;
        }

        public async Task<HttpResponseMessage> DeleteUser(string id)
        {
            return await _client.DeleteAsync($"users/{id}");
        }
    }
}
