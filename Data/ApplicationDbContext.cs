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
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<LibroCategoria> LibroCategorias { get; set; }
        public DbSet<Marcador> Marcadores { get; set; }
        public DbSet<Resaltador> Resaltadores { get; set; }

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

            modelBuilder.Entity<Marcador>()
                .HasOne(m => m.Usuario)
                .WithMany()
                .HasForeignKey(m => m.UsuarioId);

            modelBuilder.Entity<Marcador>()
                .HasOne(m => m.Libro)
                .WithMany()
                .HasForeignKey(m => m.LibroId);

            modelBuilder.Entity<Resaltador>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resaltador>()
                .HasOne(r => r.Libro)
                .WithMany()
                .HasForeignKey(r => r.LibroId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
