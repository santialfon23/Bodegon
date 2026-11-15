using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModuloUsuarioBodegon.Models
{
    public class TurnoLibre
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public TipoTurno Turno { get; set; }

        // En el módulo Usuario solo se lee; la capacidad la modifica el módulo Administrador
        public int CapacidadMaxima { get; set; }

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