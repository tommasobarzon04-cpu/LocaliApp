using LocaliApp.DTOs;
using LocaliApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LocaliApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocaliController : ControllerBase
    {
        private readonly ILocaliService _localiService;

        public LocaliController(ILocaliService localiService)
        {
            _localiService = localiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? citta)
        {
            var locali = await _localiService.GetAllAsync(citta);
            return Ok(locali);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var locale = await _localiService.GetByIdAsync(id);
            if (locale == null) return NotFound();
            return Ok(locale);
        }

        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] LocaleCreateDto dto, [FromForm] List<IFormFile>? foto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var createdLocale = await _localiService.CreateAsync(dto, userId, foto);
            return CreatedAtAction(nameof(GetById), new { id = createdLocale.Id }, createdLocale);
        }

        
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LocaleUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("L'ID della route non corrisponde all'ID del body");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isModeratore = User.IsInRole("MODERATORE");

            if (userId == null) return Unauthorized();

            var success = await _localiService.UpdateAsync(id, dto, userId, isModeratore);

            if (!success) return Forbid("Non hai i permessi per modificare questo locale o il locale non esiste.");

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isModeratore = User.IsInRole("MODERATORE");

            if (userId == null) return Unauthorized();

            var success = await _localiService.DeleteAsync(id, userId, isModeratore);

            if (!success) return Forbid("Non hai i permessi per eliminare questo locale o il locale non esiste.");

            return NoContent();
        }
    }
}
