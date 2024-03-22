using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using SistemaTurnos.Data;

namespace SistemaTurnos.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AsesorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AsesorController(AppDbContext context)
        {
            _context = context;
        }
        public class LoginRequest
        {
            public string Usuario { get; set; }
            public string Contrasena { get; set; }
        }

        [HttpGet("{jwt}")]
        [Authorize]
        public IActionResult GetAsesor(string jwt)
        {
            var asesor = _context.Asesores
                .FirstOrDefault(a => a.Jwt == jwt);

            if (asesor == null)
            {
                return NotFound("Asesor no encontrado");
            }

            return Ok(asesor);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest model)
        {
            var encryptedPassword = Encryptor.EncryptPassword(model.Contrasena);

            var asesor = _context.Asesores
                .FirstOrDefault(a => a.Username == model.Usuario && a.Password == encryptedPassword);

            if (asesor == null)
            {
                return NotFound("Usuario o contraseña incorrectos");
            }

            return Ok(asesor);
        }



        [HttpGet("{id}/turnos")]
        public IActionResult ConsultarTurnos(int id)
        {
            var turnosAsesor = _context.TurnosAtendidos
                .Where(t => t.IdAsesor == id)
                .ToList();

            return Ok(turnosAsesor);
        }
        public class EncryptPasswordRequest
        {
            public string Password { get; set; }
        }

        [HttpPost("encriptarPassword")]
        public IActionResult EncryptPassword([FromBody] EncryptPasswordRequest model)
        {
            try
            {
                var encryptedPassword = Encryptor.EncryptPassword(model.Password);
                return Ok(new { EncryptedPassword = encryptedPassword });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        public static class Encryptor
        {
            public static string EncryptPassword(string password)
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    return builder.ToString();
                }
            }
        }
    }
}
