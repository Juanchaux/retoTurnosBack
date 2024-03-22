using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.Data;
using SistemaTurnos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Models;

namespace SistemaTurnos.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TurnosAtendidosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TurnosAtendidosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("turnos-atendidos/{jwt}")]
        [Authorize]
        public IActionResult GetTurnosAtendidosHoy(string jwt)
        {
            var asesor = _context.Asesores.FirstOrDefault(a => a.Jwt == jwt);

            if (asesor == null)
            {
                return NotFound("Asesor no encontrado");
            }

            var turnosAtendidos = _context.TurnosAtendidos
                .Include(ta => ta.Turno)
                .Where(ta => ta.IdAsesor == asesor.Id && ta.Turno.HoraSolicitud.HasValue && ta.Turno.HoraSolicitud.Value.Date == DateTime.Today && ta.Turno.HoraTermino.HasValue)
                .ToList();

            return Ok(turnosAtendidos);
        }

        [HttpPost("{id}/estado")]
        public IActionResult ActualizarEstado(int id)
        {
            var turno = _context.TurnosAtendidos.FirstOrDefault(t => t.Id == id);
            if (turno == null)
            {
                return NotFound();
            }

            _context.SaveChanges();

            return Ok(turno);
        }

        [HttpGet("ultimos10Turnos")]
        public async Task<ActionResult<List<TurnosAtendidos>>> GetUltimos10Turnos()
        {
            try
            {
                var ultimos10TurnosAtendidos = await _context.TurnosAtendidos
                    .Include(t => t.Turno)
                    .OrderByDescending(t => t.Id)
                    .Take(10)
                    .ToListAsync();

                if (ultimos10TurnosAtendidos == null || ultimos10TurnosAtendidos.Count == 0)
                {
                    return NotFound("No hay ningún turno atendido registrado.");
                }

                return Ok(ultimos10TurnosAtendidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("ultimoTurnoAsesor/{jwt}")]
        [Authorize]
        public async Task<ActionResult<TurnosAtendidos>> GetUltimoTurnoAsesor(string jwt)
        {
            try
            {
                var asesor = await _context.Asesores.FirstOrDefaultAsync(a => a.Jwt == jwt);

                if (asesor == null)
                {
                    return NotFound("Asesor no encontrado.");
                }

                var ultimoTurnoAsesor = await _context.TurnosAtendidos
                    .Include(ta => ta.Turno)
                    .Where(ta => ta.IdAsesor == asesor.Id)
                    .OrderByDescending(ta => ta.Id)
                    .FirstOrDefaultAsync();

                if (ultimoTurnoAsesor == null)
                {
                    return NotFound("No hay ningún turno atendido por el asesor.");
                }

                return Ok(ultimoTurnoAsesor.Turno);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }


        [HttpGet("ultimoTurno")]
        public async Task<ActionResult<Turno>> GetUltimoTurno()
        {
            try
            {
                var ultimoTurno = await _context.Turnos.OrderByDescending(t => t.Id).FirstOrDefaultAsync();

                if (ultimoTurno == null)
                {
                    return NotFound("No hay ningún turno registrado.");
                }

                return Ok(ultimoTurno);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        public class ConsultaHistorialRequest
        {
            public string Jwt { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
        }

        [HttpPost("consultarHistorial")]
        [Authorize]
        public IActionResult ConsultarHistorias([FromBody] ConsultaHistorialRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Jwt) || request.FechaInicio == default || request.FechaFin == default)
            {
                return BadRequest("Los parámetros de consulta son inválidos.");
            }

            var asesor = _context.Asesores.FirstOrDefault(a => a.Jwt == request.Jwt);
            if (asesor == null)
            {
                return NotFound("Asesor no encontrado.");
            }

            var historialTurnos = _context.TurnosAtendidos
                .Include(ta => ta.Turno)
                .Where(ta => ta.IdAsesor == asesor.Id && ta.Turno.HoraSolicitud.HasValue && ta.Turno.HoraSolicitud.Value.Date >= request.FechaInicio.Date && ta.Turno.HoraSolicitud.Value.Date <= request.FechaFin.Date && ta.Turno.HoraTermino.HasValue)
                .ToList();

            return Ok(historialTurnos);
        }
    }
}
