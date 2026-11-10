using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModuloUsuarioBodegon.Models
{
    public class Categoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Posicion { get; set; }
        public string Icono { get; set; }

        public List<Plato> Platos { get; set; } = new List<Plato>();

        public List<Plato> ObtenerPlatos()
        {
            return new List<Plato>(Platos);
        }
    }
}