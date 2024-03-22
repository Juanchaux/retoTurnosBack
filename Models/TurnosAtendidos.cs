namespace SistemaTurnos.Models
{
    public class TurnosAtendidos
    {
        public int Id { get; set; }
        public int IdAsesor { get; set; }
        public int IdTurno { get; set; }

        public Asesor Asesor { get; set; }

        public Turno Turno { get; set; }

        public TurnosAtendidos(int idAsesor, int idTurno)
        {
            IdAsesor = idAsesor;
            IdTurno = idTurno;
        }
    }
}
