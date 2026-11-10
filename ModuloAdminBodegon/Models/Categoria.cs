using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModuloAdminBodegon.Models
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

        public bool TieneDatosValidos()
        {
            return EsNombreValido() && EsPosicionValida();
        }

        private bool EsNombreValido()
        {
            return !string.IsNullOrWhiteSpace(Nombre);
        }

        private bool EsPosicionValida()
        {
            return Posicion > 0;
        }

        public void AgregarPlato(Plato plato)
        {
            if (plato != null && !Platos.Contains(plato))
            {
                Platos.Add(plato);
            }
        }
    }
}