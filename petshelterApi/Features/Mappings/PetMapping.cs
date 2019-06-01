using AutoMapper;
using petshelterApi.Domain;
using petshelterApi.Features.Models;
using System;

namespace petshelterApi.Features.Mappings
{
    public class PetMapping : Profile
    {
        public PetMapping()
        {
            CreateMap<Pet, ReadPet>()
                .ForMember(pet => pet.Breed, opt => opt.MapFrom(src => src.Breed.Name))
                .ForMember(pet => pet.Shelter, opt => opt.MapFrom(src => src.Shelter.Name))
                .ForMember(pet => pet.Category, opt => opt.MapFrom(src => src.Breed.Category.Name))
                .ForMember(pet => pet.ExternalImage, opt => opt.MapFrom(src => src.ExternalImageUrl))
                .ForMember(pet => pet.Base64Image, opt => opt.MapFrom(src =>
                    src.Image != null ? $"data:image/{src.Image.ContentType.ToLower()};base64,{Convert.ToBase64String(src.Image.Data)}" : null));
        }
    }
}
