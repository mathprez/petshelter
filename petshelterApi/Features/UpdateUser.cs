using MediatR;
using petshelterApi.Database;
using petshelterApi.Domain.User;
using petshelterApi.Helpers;
using petshelterApi.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace petshelterApi.Features
{
    public class UpdateUser
    {
        public class Query : IRequest<Model>
        {
            public string UserId { get; set; }
        }
        public class Model
        {
            public List<Shelter> Shelters { get; set; }
            public class Shelter
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }
        }
        public class Command : IRequest<CommandResult>
        {
            public string UserId { get; set; }
            public int ShelterId { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, Model>
        {
            private readonly PetShelterDbContext _petShelterDbContext;
            private readonly IAuth0ApiClient _userManager;

            public QueryHandler(
                PetShelterDbContext petShelterDbContext,
                IAuth0ApiClient auth0ApiClient)
            {
                _petShelterDbContext = petShelterDbContext;
                _userManager = auth0ApiClient;
            }

            public async Task<Model> Handle(Query query, CancellationToken cancellationToken)
            {
                var shelters = _petShelterDbContext.Shelters.Select(x => new Model.Shelter()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

                return new Model()
                {
                    Shelters = shelters
                };
            }
        }

        public class CommandHandler : IRequestHandler<Command, CommandResult>
        {
            private readonly PetShelterDbContext _petShelterDbContext;
            private readonly IAuth0ApiClient _userManager;

            public CommandHandler(
                PetShelterDbContext petShelterDbContext,
                IAuth0ApiClient auth0ApiClient)
            {
                _petShelterDbContext = petShelterDbContext;
                _userManager = auth0ApiClient;
            }

            public async Task<CommandResult> Handle(Command command, CancellationToken cancellationToken)
            {
                var shelter = _petShelterDbContext.Shelters.FirstOrDefault(x => x.Id == command.ShelterId);

                var user = await _userManager.GetUser(command.UserId);
                var result = await _userManager.UpdateUser(new Auth0UpdateUser(user.Id, user.Connection)
                {
                    ShelterId = shelter?.Id,
                    Roles = user.Roles //command.Roles
                });

                if (result.IsSuccessStatusCode)
                {
                    return new CommandResult(System.Net.HttpStatusCode.OK, "Shelter assigned to user.", user.Id);
                }
                return new CommandResult(result.StatusCode, "Something went wrong.");

            }
        }
    }
}
