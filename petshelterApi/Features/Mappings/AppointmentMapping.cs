using AutoMapper;
using petshelterApi.Domain;

namespace petshelterApi.Features.Mappings
{
    public class AppointmentMapping : Profile
    {
        public AppointmentMapping()
        {
            CreateMap<CreateAppointment.Command, Appointment>()
                .ForMember(appt => appt.Start, opt => opt.MapFrom(src => src.Date))
                .ForMember(appt => appt.PetId, opt => opt.MapFrom(src => src.PetId));
        }
    }
}
