using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LocaliApp.Entities
{
    public class Locale
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string Indirizzo { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Citta { get; set; } = string.Empty;

        public List<Categoria> Categorie { get; set; } = new();

        public List<string> FotoUrls { get; set; } = new(); 

        
        public string? CreatorId { get; set; }
        public Utente? Creator { get; set; }

        public List<Recensione> Recensioni { get; set; } = new();
    }
}
