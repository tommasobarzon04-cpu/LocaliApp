using System.ComponentModel.DataAnnotations;

namespace LocaliApp.DTOs
{
    public class RecensioneCreateDto
    {
        public string Testo { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Valutazione { get; set; }

        public int LocaleId { get; set; }
    }

    public class RecensioneUpdateDto
    {
        public int Id { get; set; }

        public string Testo { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Valutazione { get; set; }
    }

    public class RecensioneResponseDto
    {
        public int Id { get; set; }
        public string Testo { get; set; } = string.Empty;
        public int Valutazione { get; set; }
        public string AutoreUsername { get; set; } = string.Empty;
        public string DataCreazione { get; set; } = string.Empty;
        public bool Approvata { get; set; }
    }
}
