using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using petshelterApi.Database;
using petshelterApi.Domain;
using petshelterApi.Features.Models;
using petshelterApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace petshelterApi.Features
{
    public class GetAppointments
    {
        public class Query : IRequest<Model> { }
        public class Model
        {
            public List<Appointment> Appointments { get; set; }
            public class Appointment
            {
                public ReadPet Pet { get; set; }
                public int Id { get; set; }
                public bool IsCancelled { get; set; }
                public string Date { get; set; }
                public string Year { get; set; }
                public string Start { get; set; }
                public string End { get; set; }
                public string[] ShelterAddress { get; set; }

            }
        }

        public class QueryHandler : RequestHandler<Query, Model>
        {
            private readonly PetShelterDbContext _petShelterDbContext;
            private readonly UserProfile _user;
            private readonly IMapper _mapper;

            public QueryHandler(
                PetShelterDbContext petShelterDbContext,
                UserResolverService userResolverService,
                IMapper mapper)
            {
                _petShelterDbContext = petShelterDbContext;
                _user = userResolverService.GetUser();
                _mapper = mapper;
            }

            protected override Model Handle(Query query)
            {
                var appointments = _petShelterDbContext.Appointments
                    .Include(x => x.Pet).ThenInclude(x => x.Breed).ThenInclude(x => x.Category)
                    .Include(x => x.Pet).ThenInclude(x => x.Shelter)
                    .Include(x => x.Pet).ThenInclude(x => x.Image)
                    .Where(x => x.UserId == _user.Id)
                    .OrderByDescending(x => x.Id)
                    .ToList()
                    .Select(x => new Model.Appointment
                    {
                        Pet = _mapper.Map<Pet, ReadPet>(x.Pet),
                        Id = x.Id,
                        IsCancelled = false,
                        Date = x.Start.LocalDateTime.ToString("M"),
                        Year = x.Start.LocalDateTime.ToString("yyyy"),
                        Start = x.Start.LocalDateTime.ToString("H:mm"),
                        End = x.Start.Add(new TimeSpan(1, 0, 0)).LocalDateTime.ToString("H:mm"),
                        ShelterAddress = new[] { x.Pet.Shelter.Name, x.Pet.Shelter.Address.LineOne, x.Pet.Shelter.Address.LineThree }
                    })
                    .ToList();

                return new Model()
                {
                    Appointments = appointments
                };
            }
        }
    }
}
