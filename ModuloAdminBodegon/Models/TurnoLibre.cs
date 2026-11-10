using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModuloAdminBodegon.Models
{
    public class TurnoLibre
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public TipoTurno Turno { get; set; }
        public int CapacidadMaxima { get; set; }

        // Valida y actualiza la capacidad. Devuelve false si el valor no es válido (no modifica nada)
        public bool ActualizarCapacidad(int nuevaCapacidad)
        {
            if (nuevaCapacidad <= 0)
            {
                return false;
            }

            CapacidadMaxima = nuevaCapacidad;
            return true;
        }

        // Lugares libres = capacidad máxima - personas que ya reservaron ese día y turno
        public int CalcularLugaresLibres(int personasOcupadas)
        {
            return CapacidadMaxima - personasOcupadas;
        }

        // Indica si entran las personas que se quieren reservar
        public bool HayCupo(int cantidadPersonas, int personasOcupadas)
        {
            return cantidadPersonas <= CalcularLugaresLibres(personasOcupadas);
        }
    }
}
