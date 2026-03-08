using Microsoft.EntityFrameworkCore;
using api.Model;

namespace api.Data
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Perfis>  Perfis   { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Perfis>().ToTable("Perfil");
            modelBuilder.Entity<Perfis>().HasKey(p => p.Id);
            modelBuilder.Entity<Perfis>().Property(p => p.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Usuario>().ToTable("Usuario");
            modelBuilder.Entity<Usuario>().HasKey(u => u.Id);
            modelBuilder.Entity<Usuario>().Property(u => u.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Perfil)
                .WithMany()
                .HasForeignKey(u => u.PerfilId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}