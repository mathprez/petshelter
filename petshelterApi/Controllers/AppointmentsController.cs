using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using petshelterApi.Database;
using petshelterApi.Features;
using petshelterApi.Services;
using petshelterApi.Services.Authorization;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace petshelterApi.Controllers
{
    [Route("api/appointments")]
    public class AppointmentsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly PetShelterDbContext _petShelterDbContext;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly UserResolverService _userResolverService;
        private readonly IAuthorizationService _authorizationService;

        public IBackgroundTaskQueue Queue { get; }

        public AppointmentsController(
            IMediator mediator,
            PetShelterDbContext petshelterDbContext,
            IBackgroundTaskQueue queue,
            IServiceScopeFactory serviceScopeFactory,
            UserResolverService userResolverService,
            IAuthorizationService authorizationService)
        {
            _mediator = mediator;
            _petShelterDbContext = petshelterDbContext;
            Queue = queue;
            _serviceScopeFactory = serviceScopeFactory;
            _userResolverService = userResolverService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Route("create/{petId}")]
        [Authorize]
        public async Task<IActionResult> Create(int petId)
        {
            var model = await _mediator.Send(new CreateAppointment.Query()
            {
                PetId = petId,
                UserId = _userResolverService.GetUser()?.Id
            });

            if (model == null)
            {
                return new NotFoundResult();
            }

            return Json(new { Payload = model });
        }

        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody]CreateAppointment.Command appointment)
        {
            var result = await _mediator.Send(appointment);

            if (result)
            {
                var user = _userResolverService.GetUser();
                AddMailToQueue(result.ResourceId, user);
            }

            return StatusCode((int)result.StatusCode, result.StatusText);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var model = await _mediator.Send(new GetAppointments.Query());
            return Ok(Json(model.Appointments));
        }

        [HttpDelete]
        [Route("delete/{appointmentId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int appointmentId)
        {
            var appointment = _petShelterDbContext.Appointments.FirstOrDefault(x => x.Id == appointmentId);
            if (appointment == null)
            {
                return NoContent();
            }

            if (!(await _authorizationService.AuthorizeAsync(User, appointment, Operations.Delete)).Succeeded)
            {
                return Forbid();
            }

            _petShelterDbContext.Remove(appointment);
            await _petShelterDbContext.SaveChangesAsync();
            return NoContent();
        }

        private void AddMailToQueue(object resourceId, UserProfile userProfile)
        {
            Queue.QueueBackgroundWorkItem(async token =>
            {
                var guid = Guid.NewGuid().ToString();

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var mailClient = scopedServices.GetRequiredService<MailService>();

                    try
                    {
                        var response = await mailClient.SendAppointmentMail(resourceId, userProfile);
                        Debug.WriteLine(
                           $"sending mail for appointmentId: {resourceId}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(
                            $"Task {guid}: an error occurred sending the email. Error: {ex.Message}");
                    }
                }
            });
        }
    }
}
