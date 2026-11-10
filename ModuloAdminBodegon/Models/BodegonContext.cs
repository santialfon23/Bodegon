using Microsoft.EntityFrameworkCore;

namespace ModuloAdminBodegon.Models
{
    public class BodegonContext : DbContext
    {
        public BodegonContext(DbContextOptions<BodegonContext> options) : base(options)
        {
        }

        public DbSet<Plato> Platos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<TurnoLibre> TurnosLibres { get; set; }
        public DbSet<SecuenciaReserva> SecuenciasReserva { get; set; }   // la misma tabla que usa el Módulo Usuario

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Le indica a Entity Framework que guarde el Enum como Texto (String) en TurnosLibres
            modelBuilder.Entity<TurnoLibre>()
                .Property(t => t.Turno)
                .HasConversion<string>();

            // Hacemos lo mismo para la tabla Reservas (así también guarda "Cena" en vez de 2)
            modelBuilder.Entity<Reserva>()
                .Property(r => r.Turno)
                .HasConversion<string>();
        }
    }
}