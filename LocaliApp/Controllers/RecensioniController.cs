using LocaliApp.DTOs;
using LocaliApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LocaliApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecensioniController : ControllerBase
    {
        private readonly IRecensioniService _recensioniService;

        public RecensioniController(IRecensioniService recensioniService)
        {
            _recensioniService = recensioniService;
        }

        [HttpGet("locale/{localeId}")]
        public async Task<IActionResult> GetByLocaleId(int localeId)
        {
            var recensioni = await _recensioniService.GetByLocaleIdAsync(localeId);
            return Ok(recensioni);
        }

        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RecensioneCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var result = await _recensioniService.CreateAsync(dto, userId);
            if (result == null) return BadRequest("Il locale specificato non esiste.");

            return Ok(result);
        }

       
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RecensioneUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("IDs non corrispondenti.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // Il servizio controllerà che l'userId coincida con l'autore.
            var success = await _recensioniService.UpdateAsync(id, dto, userId);

            if (!success) return Forbid("Non puoi modificare questa recensione.");

            return NoContent();
        }

        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isModeratore = User.IsInRole("MODERATORE");

            if (userId == null) return Unauthorized();

            var success = await _recensioniService.DeleteAsync(id, userId, isModeratore);

            if (!success) return Forbid("Non hai i permessi per eliminare questa recensione.");

            return NoContent();
        }

     
        [Authorize(Roles = "MODERATORE")]
        [HttpPatch("{id}/approvazione")]
        public async Task<IActionResult> ToggleApprovazione(int id, [FromQuery] bool approvata = true)
        {
            var success = await _recensioniService.ToggleApprovazioneAsync(id, approvata);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
