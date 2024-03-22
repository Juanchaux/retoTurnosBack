using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTurnos.Models
{
    public class TurnosQueue
    {
        private static List<Turno> _turnos = new List<Turno>();

        public static List<Turno> ColaTurnos => _turnos;

        public static void AgregarTurno(Turno turno)
        {
            _turnos.Add(turno);
        }

        public static List<Turno> ObtenerPrimeros5Turnos()
        {
            if (_turnos.Count == 0)
            {
                return new List<Turno>();
            }

            var primeros5Turnos = _turnos.Take(5).ToList();
            return primeros5Turnos;
        }

        public static Turno ObtenerProximoTurno()
        {
            if (_turnos.Count == 0)
            {
                return null;
            }

            var proximoTurno = _turnos[0];
            _turnos.RemoveAt(0);
            return proximoTurno;
        }

        public static void ReiniciarCola()
        {
            _turnos.Clear();
        }

        public static void OrdenarPorPrioridad()
        {
            var prioritarios = _turnos.Where(t => t.Categoria == CategoriaTurno.Prioritario).OrderBy(t => t.NumeroTurno).ToList();
            var buenaGente = _turnos.Where(t => t.Categoria == CategoriaTurno.BuenaGente).OrderBy(t => t.NumeroTurno).ToList();
            var clienteNormal = _turnos.Where(t => t.Categoria == CategoriaTurno.ClienteNormal).OrderBy(t => t.NumeroTurno).ToList();
            var turnosOrdenados = new List<Turno>();
            turnosOrdenados.AddRange(prioritarios);
            int indexBuenaGente = 0;
            int indexClienteNormal = 0;
            while (indexBuenaGente < buenaGente.Count || indexClienteNormal < clienteNormal.Count)
            {
                for (int i = 0; i < 3 && indexBuenaGente < buenaGente.Count; i++)
                {
                    turnosOrdenados.Add(buenaGente[indexBuenaGente]);
                    indexBuenaGente++;
                }

                for (int i = 0; i < 2 && indexClienteNormal < clienteNormal.Count; i++)
                {
                    turnosOrdenados.Add(clienteNormal[indexClienteNormal]);
                    indexClienteNormal++;
                }
            }
            _turnos = turnosOrdenados;
        }

        public static void MostrarColaEnConsola()
        {
            if (_turnos.Count == 0)
            {
                Console.WriteLine("La cola de turnos está vacía.");
                return;
            }

            Console.WriteLine("Cola de Turnos:");
            foreach (var turno in _turnos)
            {
                Console.WriteLine($"Numero de ID: {turno.Id} Número de Turno: {turno.NumeroTurno}, Categoría: {turno.Categoria}, Estado: {turno.Estado}");
            }
        }

    }
}
