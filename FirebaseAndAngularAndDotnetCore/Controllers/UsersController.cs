using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAndAngular.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirebaseAndAngular.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpPost("verify")]        
        public async Task<IActionResult> VerifyToken(TokenVerifyRequest request)
        {
            var auth = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance;

            try
            {
                var response = await auth.VerifyIdTokenAsync(request.Token);
                if (response != null)
                    return Accepted();
            }
            catch (FirebaseException ex)
            {
                return BadRequest();
            }

            return BadRequest();
        }

        [HttpGet("secrets")]
        [Authorize]
        public IEnumerable<string> GetSecrets()
        {
            return new List<string>()
            {
                "This is from the secret controller",
                "Seeing this means you are authenticated",
                "You have logged in using your google account from firebase",
                "Have a nice day!!"
            };
        }
    }
}