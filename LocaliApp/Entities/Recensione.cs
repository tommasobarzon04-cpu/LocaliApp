using System;
using System.ComponentModel.DataAnnotations;

namespace LocaliApp.Entities
{
    public class Recensione
    {
        public int Id { get; set; }

        public string Testo { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Valutazione { get; set; }

        public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

        
        public bool Approvata { get; set; } = true; 

        
        public string? AutoreId { get; set; }
        public Utente? Autore { get; set; }

        
        public int LocaleId { get; set; }
        public Locale? Locale { get; set; }
    }
}
