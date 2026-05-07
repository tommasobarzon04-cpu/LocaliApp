using LocaliApp.DTOs;
using LocaliApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LocaliApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(dto);
            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.LoginAsync(dto);
            if (!result.Success) return Unauthorized(result);

            return Ok(result);
        }

        
        [Authorize(Roles = "MODERATORE")]
        [HttpPost("promuovi/{username}")]
        public async Task<IActionResult> PromuoviAModeratore(string username)
        {
            var result = await _authService.PromuoviModeratoreAsync(username);
            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }
    }
}
