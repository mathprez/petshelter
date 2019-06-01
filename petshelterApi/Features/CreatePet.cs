using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using petshelterApi.Database;
using petshelterApi.Domain;
using petshelterApi.Helpers;
using petshelterApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace petshelterApi.Features
{
    public class CreatePet
    {
        public class Query : IRequest<Model> { }

        public class Model : IRequest<Command>
        {
            public List<Category> Categories { get; set; }
            public List<string> Colors { get; set; }
            public List<Gender> Genders { get; set; }
            public class Category
            {
                public int Id { get; set; }
                public string Name { get; set; }
                public List<Breed> Breeds { get; set; }
                public class Breed
                {
                    public int Id { get; set; }
                    public string Name { get; set; }
                }
            }
            public class Gender
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }
        }


        public class Command : IRequest<CommandResult>
        {
            [BindRequired]
            public IFormFile Image { get; set; }
            [BindRequired]
            public string Name { get; set; }
            [BindRequired]
            public int BreedId { get; set; }
            [BindRequired]
            public Gender Gender { get; set; }
            [BindRequired]
            public string Color { get; set; }
            [BindRequired]
            public string Description { get; set; }
        }

        public class QueryHandler : RequestHandler<Query, Model>
        {
            private readonly PetShelterDbContext _petShelterDbContext;
            public QueryHandler(PetShelterDbContext petShelterDbContext)
            {
                _petShelterDbContext = petShelterDbContext;
            }
            protected override Model Handle(Query query)
            {
                var colors = _petShelterDbContext.Pets.Select(x => x.Color).Distinct().ToList();

                var categories = _petShelterDbContext.Categories
                .Include(x => x.Breeds)
                .ToList()
                .Select(x => new Model.Category
                {
                    Id = x.Id,
                    Name = x.Name,
                    Breeds = x.Breeds.Select(b =>
                        new Model.Category.Breed
                        {
                            Id = b.Id,
                            Name = b.Name
                        }).ToList()
                })
                .ToList();

                var genders = Enum.GetValues(typeof(Gender))
                    .Cast<Gender>()
                    .Select(x =>
                    new Model.Gender
                    {
                        Id = (int)x,
                        Name = x.ToString()
                    })
                    .ToList();

                return new Model()
                {
                    Categories = categories,
                    Colors = colors,
                    Genders = genders
                };
            }
        }

        public class CommandHandler : IRequestHandler<Command, CommandResult>
        {
            private readonly PetShelterDbContext _petShelterDbContext;
            private readonly ImageSaver _imageSaver;
            private readonly UserProfile _user;

            public CommandHandler(
                PetShelterDbContext petShelterDbContext,
                ImageSaver imageSaver,
                UserResolverService userResolverService)
            {
                _petShelterDbContext = petShelterDbContext;
                _imageSaver = imageSaver;
                _user = userResolverService.GetUser();
            }

            public async Task<CommandResult> Handle(Command command, CancellationToken cancellationToken)
            {
                var breedId = _petShelterDbContext.Breeds.FirstOrDefault(x => x.Id == command.BreedId)?.Id;

                if (!breedId.HasValue)
                {
                    return new CommandResult(HttpStatusCode.BadRequest, "Invalid breed id.");
                }

                var image = await _imageSaver.SaveImage(command.Image.OpenReadStream());

                if (!_user.ShelterId.HasValue)
                {
                    return new CommandResult(HttpStatusCode.Forbidden, "User has no shelter claim.");
                }

                var petDomain = new Pet()
                {
                    Color = command.Color,
                    Description = command.Description,
                    Gender = command.Gender,
                    Name= command.Name,
                    Image = image,
                    BreedId = breedId.Value,
                    HandlerId = _user.Id,
                    ShelterId = _user.ShelterId.Value
                };

                _petShelterDbContext.Pets.Add(petDomain);                
                _petShelterDbContext.SaveChanges();

                return new CommandResult(HttpStatusCode.Created, "Pet has been created succesfully.", petDomain.Id);
            }
        }
    }
}
