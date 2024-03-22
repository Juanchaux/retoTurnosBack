using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.Utilities;

namespace SistemaTurnos.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JwtController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model");
            }

            var token = JwtUtils.GenerateJwtToken(model.Username);
            return Ok(new { Token = token });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
    }
}
