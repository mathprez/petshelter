using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using petshelterApi.Database;
using petshelterApi.Domain.User;
using petshelterApi.Features;
using petshelterApi.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace petshelterApi.Controllers
{
    [Route("api/admin")]
    public class AdminController : Controller
    {
        private readonly IAuth0ApiClient _userManager;
        private readonly IMediator _mediator;
        private readonly PetShelterDbContext _petShelterDbContext;

        public AdminController(
            IAuth0ApiClient userManager,
            IMediator mediator,
            PetShelterDbContext petShelterDbContext)
        {
            _userManager = userManager;
            _mediator = mediator;
            _petShelterDbContext = petShelterDbContext;
        }

        [HttpGet]
        [Route("users")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Users()
        {
            var usersTask = _userManager.GetUsers(new Helpers.UserParameters());
            var shelters = _petShelterDbContext.Shelters.ToList();
            var users = await usersTask;

            return Ok(Json(new
            {
                length = users.Length,
                limit = users.Limit,
                start = users.Start,
                total = users.Total,
                users = users.Users.Select(x => new
                {

                    x.Username,
                    x.ShelterId,
                    x.Email,
                    x.Roles,
                    x.Id,
                    x.Connection,
                    ShelterName = shelters.FirstOrDefault(shltr => shltr.Id == x.ShelterId.GetValueOrDefault(-1))?.Name
                })
            }));
        }

        [HttpGet]
        [Route("patch")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Patch()
        {
            var model = await _mediator.Send(new UpdateUser.Query());
            return Ok(model);
        }

        [HttpPatch]
        [Route("patch")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Patch([FromForm]UpdateUser.Command user)
        {
            var result = await _mediator.Send(user);
            return Ok();
        }
    }
}
