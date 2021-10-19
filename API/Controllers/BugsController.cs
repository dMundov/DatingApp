using System;
using API.Data;
using API.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BugsController : BaseApiController
    {
        private readonly ApplicationDbContext context;
        public BugsController(ApplicationDbContext context)
        {
            this.context = context;

        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = this.context.Users.Find(-1);

            if (thing == null) return NotFound();

            return Ok(thing);
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
                var thing = this.context.Users.Find(-1);

                var thingToReturn = thing.ToString();

                return thingToReturn;
           
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This is not a good request");
        }
    }
}