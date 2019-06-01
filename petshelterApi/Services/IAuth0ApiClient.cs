using petshelterApi.Domain.User;
using petshelterApi.Helpers;
using System.Net.Http;
using System.Threading.Tasks;

namespace petshelterApi.Services
{
    public interface IAuth0ApiClient
    {
        Task<Auth0UserList> GetUsers(UserParameters userParameters);
        Task<Auth0ReadUser> GetUser(string id);
        Task<HttpResponseMessage> UpdateUser(Auth0UpdateUser user);
        Task<HttpResponseMessage> DeleteUser(string id);
    }
}
