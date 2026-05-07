using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LocaliApp.Entities;
using Microsoft.AspNetCore.Identity;

namespace LocaliApp.Data
{
    public class AppDbContext : IdentityDbContext<Utente>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Locale> Locali { get; set; }
        public DbSet<Categoria> Categorie { get; set; }
        public DbSet<Recensione> Recensioni { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Per salvare la lista di URL come JSON nel DB SQLite (che non supporta array primitivi nativamente)
            // Stiamo utilizzando una conversione semplice per conservare la lista di stringhe
            builder.Entity<Locale>()
                .Property(l => l.FotoUrls)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new System.Collections.Generic.List<string>()
                );
        }
    }
}
