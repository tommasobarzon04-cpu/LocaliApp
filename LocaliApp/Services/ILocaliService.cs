using LocaliApp.DTOs;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocaliApp.Services
{
    public interface ILocaliService
    {
        Task<List<LocaleResponseDto>> GetAllAsync(string? citta);
        Task<LocaleResponseDto?> GetByIdAsync(int id);
        Task<LocaleResponseDto> CreateAsync(LocaleCreateDto dto, string userId, List<IFormFile>? fotoFiles);
        Task<bool> UpdateAsync(int id, LocaleUpdateDto dto, string userId, bool isModeratore);
        Task<bool> DeleteAsync(int id, string userId, bool isModeratore);
    }
}
