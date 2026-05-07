using LocaliApp.Data;
using LocaliApp.DTOs;
using LocaliApp.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocaliApp.Services
{
    public class RecensioniService : IRecensioniService
    {
        private readonly AppDbContext _context;

        public RecensioniService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RecensioneResponseDto>> GetByLocaleIdAsync(int localeId)
        {
          
            var recensioni = await _context.Recensioni
                .Include(r => r.Autore)
                .Where(r => r.LocaleId == localeId && r.Approvata == true)
                .OrderByDescending(r => r.DataCreazione)
                .ToListAsync();

            return recensioni.Select(MapToDto).ToList();
        }

        public async Task<RecensioneResponseDto?> CreateAsync(RecensioneCreateDto dto, string autoreId)
        {
            var localeExists = await _context.Locali.AnyAsync(l => l.Id == dto.LocaleId);
            if (!localeExists) return null;

            var recensione = new Recensione
            {
                Testo = dto.Testo ?? "",
                Valutazione = dto.Valutazione,
                LocaleId = dto.LocaleId,
                AutoreId = autoreId,
                Approvata = true 
            };

            _context.Recensioni.Add(recensione);
            await _context.SaveChangesAsync();

           
            recensione = await _context.Recensioni.Include(r => r.Autore).FirstAsync(r => r.Id == recensione.Id);

            return MapToDto(recensione);
        }

        public async Task<bool> UpdateAsync(int id, RecensioneUpdateDto dto, string autoreId)
        {
            var recensione = await _context.Recensioni.FirstOrDefaultAsync(r => r.Id == id);

            if (recensione == null) return false;

            
            if (recensione.AutoreId != autoreId) return false;

            recensione.Testo = dto.Testo ?? "";
            recensione.Valutazione = dto.Valutazione;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string userId, bool isModeratore)
        {
            var recensione = await _context.Recensioni.FirstOrDefaultAsync(r => r.Id == id);
            if (recensione == null) return false;

            
            if (recensione.AutoreId != userId && !isModeratore) return false;

            _context.Recensioni.Remove(recensione);
            await _context.SaveChangesAsync();
            return true;
        }

        
        public async Task<bool> ToggleApprovazioneAsync(int id, bool approvata)
        {
            var recensione = await _context.Recensioni.FirstOrDefaultAsync(r => r.Id == id);
            if (recensione == null) return false;

            recensione.Approvata = approvata;
            await _context.SaveChangesAsync();
            return true;
        }

        private RecensioneResponseDto MapToDto(Recensione recensione)
        {
            return new RecensioneResponseDto
            {
                Id = recensione.Id,
                Testo = recensione.Testo,
                Valutazione = recensione.Valutazione,
                DataCreazione = recensione.DataCreazione.ToString("yyyy-MM-dd HH:mm"),
                AutoreUsername = recensione.Autore?.UserName ?? "Utente Sconosciuto",
                Approvata = recensione.Approvata
            };
        }
    }
}
