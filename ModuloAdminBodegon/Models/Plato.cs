namespace ModuloAdminBodegon.Models
{
    public class Plato
    {

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double Precio { get; set; }
        public string ImagenUrl { get; set; }

        // Relación con Categoria (Llave foránea y propiedad de navegación)
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

        public bool TieneDatosValidos()
        {
            return EsNombreValido() && EsDescripcionValida() && EsPrecioValido();
        }

        private bool EsNombreValido()
        {
            return !string.IsNullOrWhiteSpace(Nombre);
        }

        private bool EsDescripcionValida()
        {
            return !string.IsNullOrWhiteSpace(Descripcion);
        }

        private bool EsPrecioValido()
        {
            return Precio > 0;
        }
    }
}