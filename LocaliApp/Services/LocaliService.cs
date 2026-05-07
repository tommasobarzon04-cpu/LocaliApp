using LocaliApp.Data;
using LocaliApp.DTOs;
using LocaliApp.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocaliApp.Services
{
    public class LocaliService : ILocaliService
    {
        private readonly AppDbContext _context;
        private readonly IImageService _imageService;

        public LocaliService(AppDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<List<LocaleResponseDto>> GetAllAsync(string? citta)
        {
            var query = _context.Locali
                .Include(l => l.Categorie)
                .Include(l => l.Recensioni)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(citta))
            {
                query = query.Where(l => l.Citta.ToLower() == citta.ToLower());
            }

            var locali = await query.ToListAsync();

            return locali.Select(MapToDto).ToList();
        }

        public async Task<LocaleResponseDto?> GetByIdAsync(int id)
        {
            var locale = await _context.Locali
                .Include(l => l.Categorie)
                .Include(l => l.Recensioni)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (locale == null) return null;

            return MapToDto(locale);
        }

        public async Task<LocaleResponseDto> CreateAsync(LocaleCreateDto dto, string userId, List<IFormFile>? fotoFiles)
        {
            var locale = new Locale
            {
                Nome = dto.Nome,
                Indirizzo = dto.Indirizzo,
                Citta = dto.Citta,
                CreatorId = userId
            };

          
            if (dto.CategorieIds != null && dto.CategorieIds.Any())
            {
                var categorie = await _context.Categorie.Where(c => dto.CategorieIds.Contains(c.Id)).ToListAsync();
                locale.Categorie.AddRange(categorie);
            }

           
            if (fotoFiles != null && fotoFiles.Any())
            {
                locale.FotoUrls = await _imageService.UploadImagesAsync(fotoFiles);
            }

            _context.Locali.Add(locale);
            await _context.SaveChangesAsync();

            return MapToDto(locale);
        }

        public async Task<bool> UpdateAsync(int id, LocaleUpdateDto dto, string userId, bool isModeratore)
        {
            var locale = await _context.Locali
                .Include(l => l.Categorie)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (locale == null) return false;

            
            if (locale.CreatorId != userId && !isModeratore)
                return false;

            locale.Nome = dto.Nome;
            locale.Indirizzo = dto.Indirizzo;
            locale.Citta = dto.Citta;

            
            if (dto.CategorieIds != null)
            {
                locale.Categorie.Clear();
                var categorie = await _context.Categorie.Where(c => dto.CategorieIds.Contains(c.Id)).ToListAsync();
                locale.Categorie.AddRange(categorie);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string userId, bool isModeratore)
        {
             var locale = await _context.Locali.FirstOrDefaultAsync(l => l.Id == id);
             if (locale == null) return false;

             
             if (locale.CreatorId != userId && !isModeratore) return false;

             _context.Locali.Remove(locale);
             await _context.SaveChangesAsync();
             return true;
        }

        
        private LocaleResponseDto MapToDto(Locale locale)
        {
            double media = 0;
            if (locale.Recensioni != null && locale.Recensioni.Any())
            {
                media = locale.Recensioni.Average(r => r.Valutazione);
            }

            return new LocaleResponseDto
            {
                Id = locale.Id,
                Nome = locale.Nome,
                Indirizzo = locale.Indirizzo,
                Citta = locale.Citta,
                CreatorId = locale.CreatorId ?? "",
                FotoUrls = locale.FotoUrls,
                Categorie = locale.Categorie?.Select(c => c.Nome).ToList() ?? new List<string>(),
                MediaValutazioni = media
            };
        }
    }
}
