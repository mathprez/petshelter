using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using petshelterApi.Database;
using petshelterApi.Features;
using System.Linq;
using System.Threading.Tasks;

namespace petshelterApi.Controllers
{
    [Route("api/pets")]
    public class PetsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly PetShelterDbContext _petShelterDbContext;
        public PetsController(IMediator mediator, PetShelterDbContext petShelterDbContext)
        {
            _mediator = mediator;
            _petShelterDbContext = petShelterDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery]GetPets.Query query)
        {
            var model = await _mediator.Send(query);
            return Json(new { Payload = model });
        }

        [HttpGet]
        [Route("searchData")]
        public IActionResult SearchData()
        {
            var categories = _petShelterDbContext.Categories
                .Include(x => x.Breeds)
                .ToList()
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    Breeds = x.Breeds.Select(b =>
                        new
                        {
                            b.Id,
                            b.Name
                        })
                });

            var shelters = _petShelterDbContext.Shelters.Select(x => new
            {
                x.Id,
                x.Name
            });

            return Json(new { Categories = categories, Shelters =shelters });
        }

        [HttpGet]
        [Route("create")]
        [Authorize(Roles = "volunteer, admin")]
        public async Task<IActionResult> Create()
        {
            var model = await _mediator.Send(new CreatePet.Query());
            return Ok(Json(model));

        }

        [HttpPost]
        [Route("create")]
        [Authorize(Roles = "volunteer, admin")]
        public async Task<IActionResult> Create([FromForm]CreatePet.Command pet)
        {
            var result = await _mediator.Send(pet);
            return StatusCode((int)result.StatusCode, result.StatusText);
        }
    }
}
