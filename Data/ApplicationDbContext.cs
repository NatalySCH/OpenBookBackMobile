using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using OpenBooksBackMobile.Entities;
using Microsoft.EntityFrameworkCore;

namespace OpenBooksBackMobile.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Libro> Libros { get; set; }
        public DbSet<UsuarioLibro> UsuarioLibros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UsuarioLibro>()
                .HasOne(ul => ul.Usuario)
                .WithMany(u => u.Libros)
                .HasForeignKey(ul => ul.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsuarioLibro>()
                .HasOne(ul => ul.Libro)
                .WithMany(l => l.Usuarios)
                .HasForeignKey(ul => ul.LibroId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsuarioLibro>()
                .HasIndex(ul => new { ul.UsuarioId, ul.LibroId })
                .IsUnique();

            modelBuilder.Entity<Libro>()
                .HasOne(l => l.UsuarioCreador)
                .WithMany()
                .HasForeignKey(l => l.UsuarioCreadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LibroCategoria>()
                .HasKey(lc => new { lc.LibroId, lc.CategoriaId });

            modelBuilder.Entity<LibroCategoria>()
                .HasOne(lc => lc.Libro)
                .WithMany(l => l.LibroCategorias)
                .HasForeignKey(lc => lc.LibroId);

            modelBuilder.Entity<LibroCategoria>()
                .HasOne(lc => lc.Categoria)
                .WithMany(c => c.LibroCategorias)
                .HasForeignKey(lc => lc.CategoriaId);
        }
    }
}
