using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModuloUsuarioBodegon.Models
{
    public class Categoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int Posicion { get; set; }
        public string Icono { get; set; } = null!;

        public List<Plato> Platos { get; set; } = new List<Plato>();

        public List<Plato> ObtenerPlatos()
        {
            return new List<Plato>(Platos);
        }
    }
}