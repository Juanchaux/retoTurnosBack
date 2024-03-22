using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;
using SistemaTurnos.Data;
using SistemaTurnos.Models;

namespace SistemaTurnos.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TurnoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TurnoController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("turnosNoAtendidos")]
        [Authorize]
        public async Task<ActionResult<List<Turno>>> GetTurnosNoAtendidos()
        {
            var turnosNoAtendidos = await _context.Turnos
                .Where(t => t.Estado == "NO ATENDIDO")
                .ToListAsync();

            return Ok(turnosNoAtendidos);
        }


        [HttpPost("llamarTurnoNoAtendido")]
        [Authorize]
        public async Task<IActionResult> LlamarTurnoNoAtendido([FromBody] TurnoRequest model)
        {
            try
            {
                var turno = await _context.Turnos.FirstOrDefaultAsync(t => t.Id == model.TurnoId);

                if (turno == null)
                {
                    return NotFound($"No se encontró el turno con ID {model.TurnoId}");
                }

                if (turno.Estado == "LLAMADO")
                {
                    return BadRequest($"El turno con ID {model.TurnoId} ya está marcado como llamado");
                }

                if (turno.Estado == "TERMINADO")
                {
                    return BadRequest($"El turno con ID {model.TurnoId} ya está marcado como terminado");
                }
                turno.Estado = "LLAMADO";
                TurnosQueue.AgregarTurno(turno);
                TurnosQueue.OrdenarPorPrioridad();
                await _context.SaveChangesAsync();

                return Ok(new { message = "Estado cambiado con exito" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("primeros5Turnos")]
        [Authorize]
        public ActionResult<List<Turno>> Primeros5Turnos()
        {
            var ultimos5Turnos = TurnosQueue.ObtenerPrimeros5Turnos();

            if (ultimos5Turnos.Count == 0)
            {
                return NotFound("No hay turnos disponibles.");
            }

            return Ok(ultimos5Turnos);
        }

        [HttpGet("siguienteTurno/{jwt}")]
        [Authorize]
        public async Task<IActionResult> SiguienteTurno(string jwt)
        {
            try
            {
                var asesor = _context.Asesores
               .FirstOrDefault(a => a.Jwt == jwt);

                var proximoTurno = TurnosQueue.ObtenerProximoTurno();
                TurnosQueue.OrdenarPorPrioridad();

                if (proximoTurno == null)
                {
                    return NotFound("No hay turnos disponibles en la cola.");
                }

                proximoTurno.HoraLlamada = DateTime.Now;
                proximoTurno.Estado = "LLAMADO";
                _context.Update(proximoTurno);
                await _context.SaveChangesAsync();

                var turnoAtendido = new TurnosAtendidos(asesor.Id, proximoTurno.Id);

                _context.TurnosAtendidos.Add(turnoAtendido);
                await _context.SaveChangesAsync();

                return Ok(new { Turno = proximoTurno });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        public class TurnoRequest
        {
            public int TurnoId { get; set; }
        }

        [HttpPost("terminarTurno")]
        [Authorize]
        public async Task<IActionResult> TerminarTurno([FromBody] TurnoRequest model)
        {
            try
            {
                var turno = await _context.Turnos.FirstOrDefaultAsync(t => t.Id == model.TurnoId);
                if (turno == null)
                {
                    return NotFound(new { message = $"No se encontró el turno con ID {model.TurnoId}" });
                }

                if (turno.Estado == "TERMINADO")
                {
                    return BadRequest(new { message = $"El turno con ID {model.TurnoId} ya está marcado como terminado" });
                }

                turno.Estado = "TERMINADO";
                turno.HoraTermino = DateTime.Now;

                await _context.SaveChangesAsync();
                return Ok(new { message = $"El turno con ID {model.TurnoId} ha sido marcado como terminado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error interno del servidor: {ex.Message}" });
            }
        }



        [HttpPost("noAtendido")]
        [Authorize]
        public async Task<IActionResult> MarcarNoAtendido([FromBody] TurnoRequest model)
        {
            try
            {
                var turno = await _context.Turnos.FirstOrDefaultAsync(t => t.Id == model.TurnoId);

                if (turno == null)
                {
                    return NotFound($"No se encontró el turno con ID {model.TurnoId}");
                }

                if (turno.Estado == "NO ATENDIDO")
                {
                    return BadRequest($"El turno con ID {model.TurnoId} ya está marcado como no atendido");
                }

                if (turno.Estado == "TERMINADO")
                {
                    return BadRequest($"El turno con ID {model.TurnoId} ya está marcado como terminado");
                }
                turno.Estado = "NO ATENDIDO";
                await _context.SaveChangesAsync();

                return Ok(new { message = "Estado cambiado con exito" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("solicitarTurno")]
        public async Task<IActionResult> SolicitarTurno([FromBody] int categoria)
        {
            try
            {

                var ultimoTurnoDia = await _context.Turnos
                    .OrderByDescending(t => t.HoraSolicitud)
                    .FirstOrDefaultAsync();

                if (ultimoTurnoDia != null && ultimoTurnoDia.HoraSolicitud.Value.Date != DateTime.Today)
                {
                    TurnosQueue.ReiniciarCola();
                }

                if (categoria < 0 || categoria > 2)
                {
                    return BadRequest("La categoría del turno debe ser un valor entre 0 y 2");
                }

                var fechaActual = DateTime.Today;
                var ultimoTurno = await _context.Turnos
                    .Where(t => t.HoraSolicitud.HasValue && t.HoraSolicitud.Value.Date == fechaActual && t.Categoria == (CategoriaTurno)categoria)
                    .OrderByDescending(t => t.NumeroTurno)
                    .FirstOrDefaultAsync();

                var nuevoNumeroTurno = ultimoTurno == null ? 1 : ultimoTurno.NumeroTurno + 1;
                var ultimoId = await _context.Turnos
                    .OrderByDescending(t => t.Id)
                    .FirstOrDefaultAsync();

                var turno = new Turno(ultimoId == null ? 1 : ultimoId.Id + 1, categoria == 0 ? CategoriaTurno.Prioritario : categoria == 1 ? CategoriaTurno.BuenaGente : CategoriaTurno.ClienteNormal, nuevoNumeroTurno, DateTime.Now);
                _context.Turnos.Add(turno);
                TurnosQueue.AgregarTurno(turno);
                TurnosQueue.OrdenarPorPrioridad();
                await _context.SaveChangesAsync();

                return Ok(new { Turno = turno });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
