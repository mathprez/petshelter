using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using petshelterApi.Domain;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace petshelterApi.Services.Authorization
{
    public class AppointmentAuthorizationCrudHandler : AuthorizationHandler<OperationAuthorizationRequirement, Appointment>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   OperationAuthorizationRequirement requirement,
                                                   Appointment resource)
        {
            if (context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value == resource.UserId &&
                requirement.Name == Operations.Delete.Name)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
