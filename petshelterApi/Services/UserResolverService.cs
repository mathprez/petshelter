using Microsoft.AspNetCore.Http;
using petshelterApi.Helpers;
using System.Linq;
using System.Security.Claims;

namespace petshelterApi.Services
{
    public class UserResolverService
    {
        private readonly IHttpContextAccessor _context;
        public UserResolverService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public UserProfile GetUser()
        {
            var user = _context.HttpContext.User;

            var userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            int? shelterId = null;
            if (int.TryParse(user.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Shelter)?.Value, out var result))
            {
                shelterId = result;
            }

            return new UserProfile()
            {
                IsAuthenticated = user.Identity.IsAuthenticated,
                Email = email,
                Id = userId,
                ShelterId = shelterId
            };
        }
    }

    public class UserProfile
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool IsAuthenticated { get; set; }
        public int? ShelterId { get; set; }
    }
}
