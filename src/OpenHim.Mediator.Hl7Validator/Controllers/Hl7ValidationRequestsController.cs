using Microsoft.AspNetCore.Mvc;

namespace OpenHim.Mediator.Hl7Validator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Hl7ValidationRequestsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post(string hl7Message)
        {
            return Ok();
        }
    }
}