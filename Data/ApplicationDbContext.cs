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
        public DbSet<Valoracion> Valoraciones { get; set; }
        public DbSet<Resena> Resenas { get; set; }
        public DbSet<Sugerencia> Sugerencias { get; set; }
        public DbSet<Denuncia> Denuncias { get; set; }

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

            modelBuilder.Entity<Valoracion>(entity =>
            {
                entity.ToTable("Valoraciones");
                entity.HasKey(v => v.Id);

                entity.Property(v => v.Puntuacion)
                    .IsRequired();

                entity.Property(v => v.Fecha)
                    .IsRequired();

                entity.HasOne(v => v.Usuario)
                    .WithMany()
                    .HasForeignKey(v => v.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(v => v.Libro)
                    .WithMany(l => l.Valoraciones)
                    .HasForeignKey(v => v.LibroId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(v => new { v.UsuarioId, v.LibroId })
                    .IsUnique();

            });
            modelBuilder.Entity<Resena>(entity =>
            {
                entity.ToTable("Resenas");

                entity.HasKey(r => r.Id);

                entity.HasOne(r => r.Usuario)
                      .WithMany()
                      .HasForeignKey(r => r.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Libro)
                      .WithMany(l => l.Resenas)
                      .HasForeignKey(r => r.LibroId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(r => r.Texto)
                      .IsRequired()
                      .HasMaxLength(2000);

                entity.Property(r => r.Fecha)
                      .IsRequired();
            });
            modelBuilder.Entity<Sugerencia>(entity =>
            {
                entity.ToTable("Sugerencias");

                entity.HasKey(s => s.Id);

                entity.Property(s => s.Comentario)
                    .IsRequired()
                    .HasMaxLength(2000);

               
                entity.HasOne(s => s.Usuario)
                    .WithMany() 
                    .HasForeignKey(s => s.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(s => s.UsuarioId);
            });

            modelBuilder.Entity<Denuncia>(entity =>
            {
                entity.ToTable("Denuncias");

                entity.HasKey(d => d.Id);

                entity.Property(d => d.Comentario)
                    .IsRequired()
                    .HasMaxLength(1000);

                // 🔹 Relación: Denunciante
                entity.HasOne(d => d.UsuarioDenunciante)
                    .WithMany()
                    .HasForeignKey(d => d.IdDenunciante)
                    .OnDelete(DeleteBehavior.Restrict);

                // 🔹 Relación: Denunciado
                entity.HasOne(d => d.UsuarioDenunciado)
                    .WithMany()
                    .HasForeignKey(d => d.IdDenunciado)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
