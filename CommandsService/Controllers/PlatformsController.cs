using Microsoft.AspNetCore.Mvc;
using System;

namespace CommandsService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        public PlatformsController()
        {

        }

        [HttpPost]
        public ActionResult TestInboudConnection() 
        {
            Console.WriteLine("--> Inbound post # Command Service");

            return Ok("Inbound test of from Platforms Controller");
        }
    }
}
