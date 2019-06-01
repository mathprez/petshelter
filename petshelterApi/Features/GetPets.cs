using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using petshelterApi.Database;
using petshelterApi.Domain;
using petshelterApi.Features.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace petshelterApi.Features
{
    public class GetPets
    {
        public class Query : IRequest<Model>
        {
            const int maxPageSize = 100;
            public int PageNumber { get; set; } = 1;
            private int _pageSize = 10;
            public int PageSize
            {
                get => _pageSize > 0 ? _pageSize : 10;
                set => _pageSize = (value > maxPageSize && value > 0) ? maxPageSize : value;
            }
            public int CategoryId { get; set; }
            public int BreedId { get; set; }
            public int GenderId { get; set; }
            public int ShelterId { get; set; }
        }

        public class Model
        {
            public int Count { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int PageCount => (int)Math.Ceiling((decimal)Count / PageSize);
            public List<ReadPet> Pets { get; set; }
        }

        public class QueryHandler : RequestHandler<Query, Model>
        {
            private readonly PetShelterDbContext _petShelterDbContext;
            private readonly IMapper _mapper;

            public QueryHandler(PetShelterDbContext petShelterDbContext, IMapper mapper)
            {
                _petShelterDbContext = petShelterDbContext;
                _mapper = mapper;
            }

            protected override Model Handle(Query query)
            {
                var petsDomain = GetDbQuery(query, out var count).ToList();
                               
                return new Model
                {
                    Count = count,
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize,
                    Pets = _mapper.Map<List<Pet>, List<ReadPet>>(petsDomain)
                };
            }
            
            private IQueryable<Pet> GetDbQuery(Query query, out int count)
            {
                IQueryable<Pet> dbQuery = _petShelterDbContext.Pets
                    .Include(x => x.Shelter)
                    .Include(x => x.Breed)
                    .Include(x => x.Image)
                    .Include(x => x.Breed.Category);

                var gender = query.GenderId == 1 ? Gender.Male : Gender.Female;
                if (query.GenderId > 0)
                {
                    dbQuery = dbQuery.Where(x => x.Gender == gender);
                }

                if (query.ShelterId > 0)
                {
                    dbQuery = dbQuery.Where(x => x.ShelterId == query.ShelterId);
                }

                if (query.CategoryId > 0)
                {
                    dbQuery = dbQuery.Where(x => x.Breed.Category.Id == query.CategoryId);

                    if (query.BreedId > 0)
                    {
                        dbQuery = dbQuery.Where(x => x.BreedId == query.BreedId);
                    }
                }
                
                //weird dog
                dbQuery = dbQuery.Where(x => x.BreedId != 219);
                
                count = dbQuery.Count();
                if (query.PageNumber == 0)
                {
                    query.PageNumber = 1;
                }
                dbQuery = dbQuery
                    .Skip((query.PageNumber - 1) * query.PageSize)
                    .Take(query.PageSize);
                
                return dbQuery;
            }
        }
    }
}
