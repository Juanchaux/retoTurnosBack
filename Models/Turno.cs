using System;

namespace SistemaTurnos.Models
{
    public enum CategoriaTurno
    {
        Prioritario,
        BuenaGente,
        ClienteNormal
    }

    public class Turno
    {
        public int Id { get; set; }
        public CategoriaTurno Categoria { get; set; }
        public int NumeroTurno { get; set; }
        public string Estado { get; set; }
        public DateTime? HoraSolicitud { get; set; }
        public DateTime? HoraLlamada { get; set; }
        public DateTime? HoraTermino { get; set; }

        public Turno(CategoriaTurno categoria, int numeroTurno, DateTime? horaSolicitud)
        {
            Categoria = categoria;
            NumeroTurno = numeroTurno;
            Estado = "ESPERA";
            HoraSolicitud = horaSolicitud;
        }
        public Turno(int id, CategoriaTurno categoria, int numeroTurno, DateTime? horaSolicitud)
        {
            Id = id;
            Categoria = categoria;
            NumeroTurno = numeroTurno;
            Estado = "ESPERA";
            HoraSolicitud = horaSolicitud;
        }
    }
}
