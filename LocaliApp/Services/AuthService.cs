using LocaliApp.DTOs;
using LocaliApp.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LocaliApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Utente> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<Utente> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var userExists = await _userManager.FindByEmailAsync(dto.Email);
            if (userExists != null)
                return new AuthResponseDto { Success = false, Message = "Email già in uso." };

            var usernameExists = await _userManager.FindByNameAsync(dto.Username);
            if (usernameExists != null)
                return new AuthResponseDto { Success = false, Message = "Username già in uso." };

            var user = new Utente
            {
                Email = dto.Email,
                UserName = dto.Username,
                SecurityStamp = Guid.NewGuid().ToString() 
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponseDto { Success = false, Message = $"Errore di registrazione: {errors}" };
            }

            
            if (!await _roleManager.RoleExistsAsync("UTENTE"))
                await _roleManager.CreateAsync(new IdentityRole("UTENTE"));
            if (!await _roleManager.RoleExistsAsync("MODERATORE"))
                await _roleManager.CreateAsync(new IdentityRole("MODERATORE"));

            
            await _userManager.AddToRoleAsync(user, "UTENTE");

            return new AuthResponseDto { Success = true, Message = "Registrazione completata. Ora puoi effettuare il login." };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return new AuthResponseDto { Success = false, Message = "Credenziali non valide." };

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id), 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GetToken(authClaims);

            return new AuthResponseDto
            {
                Success = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Message = "Login effettuato con successo."
            };
        }

        public async Task<AuthResponseDto> PromuoviModeratoreAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return new AuthResponseDto { Success = false, Message = "Utente non trovato." };

            if (!await _roleManager.RoleExistsAsync("MODERATORE"))
                await _roleManager.CreateAsync(new IdentityRole("MODERATORE"));

            if (await _userManager.IsInRoleAsync(user, "MODERATORE"))
                return new AuthResponseDto { Success = false, Message = "L'utente è già un Moderatore." };

           
            var result = await _userManager.AddToRoleAsync(user, "MODERATORE");

            if (!result.Succeeded)
                return new AuthResponseDto { Success = false, Message = "Impossibile promuovere l'utente." };

            return new AuthResponseDto { Success = true, Message = $"L'utente {username} è diventato un MODERATORE." };
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfiguration:Secret"]!));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtConfiguration:Issuer"],
                audience: _configuration["JwtConfiguration:Audience"],
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtConfiguration:ExpirationInMinutes"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}
