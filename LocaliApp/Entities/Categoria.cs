using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LocaliApp.Entities
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        public List<Locale> Locali { get; set; } = new();
    }
}
