using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModuloAdminBodegon.Models
{
    public class Reserva
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string NumeroReserva { get; set; }
        public DateTime Fecha { get; set; }
        public int CantidadPersonas { get; set; }
        public TipoTurno Turno { get; set; }

        // Toda reserva nueva nace Activa. Al cancelarla no se borra: cambia de estado (borrado lógico)
        public EstadoReserva Estado { get; set; } = EstadoReserva.Activa;

        // Relación con el Cliente
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public bool EsValida()
        {
            return EsFechaValida() && EsCantidadPersonasValida();
        }

        // La fecha debe ser hoy o futura y no puede ser lunes (local cerrado)
        private bool EsFechaValida()
        {
            return Fecha.Date >= DateTime.Today && Fecha.DayOfWeek != DayOfWeek.Monday;
        }

<<<<<<< HEAD
<<<<<<< HEAD
        // Al menos 1 persona. El máximo lo controla el cupo del turno (TurnoLibre.HayCupo)
=======
=======
>>>>>>> b74edd4add84fd31f8a387c2371a5376f0d8560e
        public void CompletarSiYaPaso()
        {
            if (Fecha < DateTime.Now)
            {
                Estado = EstadoReserva.Completada;
            }
        }

        // Al menos 1 persona. El máximo lo controla el cupo del turno (TurnoLibre, en la base)
<<<<<<< HEAD
>>>>>>> b74edd4add84fd31f8a387c2371a5376f0d8560e
=======
>>>>>>> b74edd4add84fd31f8a387c2371a5376f0d8560e
        private bool EsCantidadPersonasValida()
        {
            return CantidadPersonas > 0;
        }
<<<<<<< HEAD
<<<<<<< HEAD
=======
=======
>>>>>>> b74edd4add84fd31f8a387c2371a5376f0d8560e

        // La reserva no se elimina, pasa a estado Cancelada
        public void Cancelar()
        {
            Estado = EstadoReserva.Cancelada;
        }
<<<<<<< HEAD
>>>>>>> b74edd4add84fd31f8a387c2371a5376f0d8560e
=======
>>>>>>> b74edd4add84fd31f8a387c2371a5376f0d8560e

        // Recibe la secuencia, le pide el número siguiente y lo guarda como número de esta reserva
        public void AsignarNumero(SecuenciaReserva secuencia)
        {
            NumeroReserva = secuencia.GenerarNumeroReserva();
        }
<<<<<<< HEAD
<<<<<<< HEAD

        // Borrado lógico: solo se pueden cancelar reservas activas. Devuelve false si no se pudo
        public bool Cancelar()
        {
            if (Estado != EstadoReserva.Activa)
            {
                return false;
            }

            Estado = EstadoReserva.Cancelada;
            return true;
        }

        // Si la reserva sigue activa y su día ya pasó, pasa a Completada. Devuelve true si cambió
        public bool CompletarSiYaPaso()
        {
            if (Estado == EstadoReserva.Activa && Fecha.Date < DateTime.Today)
            {
                Estado = EstadoReserva.Completada;
                return true;
            }

            return false;
        }
=======
>>>>>>> b74edd4add84fd31f8a387c2371a5376f0d8560e
=======
>>>>>>> b74edd4add84fd31f8a387c2371a5376f0d8560e
    }
}