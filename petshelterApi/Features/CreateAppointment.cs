using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using petshelterApi.Database;
using petshelterApi.Domain;
using petshelterApi.Features.Models;
using petshelterApi.Helpers;
using petshelterApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace petshelterApi.Features
{
    public class CreateAppointment
    {
        private static readonly TimeSpan _localMinTime = new TimeSpan(9, 15, 0);
        private static readonly TimeSpan _localMaxTime = new TimeSpan(17, 0, 0);
        private static readonly TimeSpan _apptDuration = new TimeSpan(1, 0, 0);
        private static readonly TimeSpan _margin = new TimeSpan(0, 30, 0);

        public class Query : IRequest<Model>
        {
            public int PetId { get; set; }
            public string UserId { get; set; }
        }

        public class Model
        {
            public ReadPet Pet { get; set; }
            public Day AvailableDayTemplate { get; set; }
            public Day[] AppointmentDays { get; set; }
            public class Day
            {
                public DateTime? Date { get; set; }
                public Slot[] Slots { get; set; }
                public class Slot
                {
                    public TimeSpan Start { get; set; }
                    public TimeSpan End { get; set; }
                    public bool Available { get; set; }
                }
            }
        }

        public class Command : IRequest<CommandResult>
        {
            public int PetId { get; set; }
            public DateTimeOffset Date { get; set; }
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
                var pet = _petShelterDbContext.Pets
                    .Include(x => x.Shelter)
                    .Include(x => x.Breed.Category)
                    .FirstOrDefault(x => x.Id == query.PetId);
                if (pet == null) return null;

                var bookedAppointments = _petShelterDbContext.Appointments
                    .Include(x => x.Pet)
                    .Where(x => (x.UserId == query.UserId ||
                                x.Pet.HandlerId == pet.HandlerId) &&
                                x.Start.LocalDateTime.Date >= DateTime.Now.Date)
                    .ToList();

                return new Model()
                {
                    AvailableDayTemplate = new Model.Day() { Slots = GetSlots().ToArray() },
                    AppointmentDays = GetAppointmentDays(bookedAppointments, query).ToArray(),
                    Pet = _mapper.Map<Pet, ReadPet>(pet)
                };
            }

            private IEnumerable<Model.Day> GetAppointmentDays(List<Appointment> appointments, Query query)
            {
                foreach (var byDate in appointments.GroupBy(x => x.Start.LocalDateTime.Date))
                {
                    yield return new Model.Day()
                    {
                        Date = byDate.Key,
                        Slots = GetSlots(byDate).ToArray()
                    };
                }
            }

            private IEnumerable<Model.Day.Slot> GetSlots(IGrouping<DateTime, Appointment> appointmentsForDay = null)
            {
                var totalDuration = _apptDuration.Add(_margin);
                for (var x = _localMinTime; x.Add(totalDuration) < _localMaxTime; x = x.Add(totalDuration))
                {
                    yield return new Model.Day.Slot()
                    {
                        Start = x.Add(_margin / 2),
                        End = x.Add(totalDuration).Subtract(_margin / 2),
                        Available = appointmentsForDay == null || !Overlap(x, x.Add(totalDuration), appointmentsForDay)
                    };
                }
            }
        }

        public class CommandHandler : RequestHandler<Command, CommandResult>
        {
            private readonly PetShelterDbContext _petShelterDbContext;
            private readonly IMapper _mapper;
            private readonly UserProfile _userProfile;

            public CommandHandler(PetShelterDbContext petShelterDbContext, IMapper mapper, UserResolverService userResolverService)
            {
                _petShelterDbContext = petShelterDbContext;
                _mapper = mapper;
                _userProfile = userResolverService.GetUser();
            }

            protected override CommandResult Handle(Command command)
            {
                var pet = _petShelterDbContext.Pets.FirstOrDefault(x => x.Id == command.PetId);
                if (pet == null)
                {
                    return new CommandResult(HttpStatusCode.NotFound);
                }

                if (command.Date < DateTime.UtcNow)
                {
                    return new CommandResult(
                        HttpStatusCode.UnprocessableEntity,
                        "The requested datetime is in the past.");
                }

                var bookedAppointments = _petShelterDbContext.Appointments
                    .Where(x => x.Start.Date > DateTime.UtcNow &&
                                (x.Pet.HandlerId == pet.HandlerId ||
                                x.UserId == _userProfile.Id))
                                .ToList();

                // commented for testing purposes

                //if (bookedAppointments.Any(x => x.PetId == pet.Id && x.UserId == _userProfile.Id))
                //{
                //    return new CommandResult(
                //     HttpStatusCode.UnprocessableEntity,
                //     "User already has appointment with pet.");
                //}

                if (Overlap(
                    command.Date.LocalDateTime.TimeOfDay, 
                    command.Date.LocalDateTime.Add(_apptDuration).TimeOfDay, 
                    bookedAppointments.Where(x => x.Start.LocalDateTime.Date == command.Date.LocalDateTime.Date)))
                {
                    return new CommandResult(
                        HttpStatusCode.UnprocessableEntity,
                        "The requested datetime overlaps with another appointment.");
                }

                if (command.Date.LocalDateTime.TimeOfDay < _localMinTime ||
                    command.Date.LocalDateTime.Add(_apptDuration).TimeOfDay > _localMaxTime)
                {
                    return new CommandResult(
                        HttpStatusCode.UnprocessableEntity,
                        "The requested datetime is outside business hours.");
                }

                var appointment = _mapper.Map<Command, Appointment>(command);
                appointment.UserId = _userProfile.Id;

                _petShelterDbContext.Appointments.Add(appointment);
                _petShelterDbContext.SaveChanges();

                return new CommandResult(
                    HttpStatusCode.OK,
                    "The appointment has been created succesfully.",
                    appointment.Id);
            }
        }

        private static bool Overlap(TimeSpan start, TimeSpan end, IEnumerable<Appointment> appointmentsForDay)
        {
            var startTimes = appointmentsForDay.Select(x => x.Start.LocalDateTime.TimeOfDay);
            foreach (var startTime in startTimes)
            {
                var startTimeFrom = startTime.Subtract(_margin / 2);
                var startTimeUntil = startTime.Add(_apptDuration).Add(_margin / 2);
                if (startTimeFrom < end && startTimeUntil > start)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
