using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModuloUsuarioBodegon.Models
{
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }

        // Propiedad de navegación: Un cliente puede tener varias reservas asociadas
        public List<Reserva> Reservas { get; set; } = new List<Reserva>();

        public bool TieneDatosValidos()
        {
            return EsNombreValido() && EsTelefonoValido() && EsEmailValido();
        }

        private bool EsNombreValido()
        {
            return !string.IsNullOrWhiteSpace(Nombre);
        }

        private bool EsTelefonoValido()
        {
            return !string.IsNullOrWhiteSpace(Telefono);
        }

        private bool EsEmailValido()
        {
            return !string.IsNullOrWhiteSpace(Email) && Email.Contains("@");
        }

        public bool EsMismoCliente(string nombre, string email, string telefono)
        {
            return this.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase) &&
                   this.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                   this.Telefono == telefono;
        }
    }
}