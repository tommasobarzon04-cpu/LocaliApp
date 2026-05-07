using LocaliApp.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocaliApp.Services
{
    public interface IRecensioniService
    {
        Task<List<RecensioneResponseDto>> GetByLocaleIdAsync(int localeId);
        Task<RecensioneResponseDto?> CreateAsync(RecensioneCreateDto dto, string autoreId);
        Task<bool> UpdateAsync(int id, RecensioneUpdateDto dto, string autoreId);
        Task<bool> DeleteAsync(int id, string userId, bool isModeratore);

      
        Task<bool> ToggleApprovazioneAsync(int id, bool approvata);
    }
}
