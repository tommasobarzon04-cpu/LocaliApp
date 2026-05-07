using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LocaliApp.DTOs
{
    public class LocaleCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string Indirizzo { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Citta { get; set; } = string.Empty;

        // IDs delle categorie da associare
        public List<int> CategorieIds { get; set; } = new();
    }

    public class LocaleUpdateDto : LocaleCreateDto
    {
        public int Id { get; set; }
    }

    public class LocaleResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Indirizzo { get; set; } = string.Empty;
        public string Citta { get; set; } = string.Empty;
        public string CreatorId { get; set; } = string.Empty;
        public List<string> Categorie { get; set; } = new();
        public List<string> FotoUrls { get; set; } = new();
        public double MediaValutazioni { get; set; }
    }
}
