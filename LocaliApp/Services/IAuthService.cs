using LocaliApp.DTOs;
using System.Threading.Tasks;

namespace LocaliApp.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> PromuoviModeratoreAsync(string username);
    }
}
