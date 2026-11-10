using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModuloUsuarioBodegon.Models
{
    public class Plato
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double Precio { get; set; }
        public string ImagenUrl { get; set; }

        // Relación con Categoria (Llave foránea y propiedad de navegación)
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
    }
}

