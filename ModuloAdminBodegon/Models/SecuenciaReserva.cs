using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModuloAdminBodegon.Models
{
    public class SecuenciaReserva
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UltimoNumero { get; set; }

        // Suma 1 al último número y lo devuelve con formato #NNNN
        public string GenerarNumeroReserva()
        {
            UltimoNumero++;
            return "#" + UltimoNumero;
        }
    }
}
