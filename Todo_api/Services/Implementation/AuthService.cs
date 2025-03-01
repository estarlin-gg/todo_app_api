using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Todo_api.Models;
using Todo_api.Services.Abstraction;

namespace Todo_api.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<AplicationUser> _signInManager;

        public AuthService(UserManager<AplicationUser> userManager, SignInManager<AplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<string> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || !(await _userManager.CheckPasswordAsync(user, password)))
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

        
            var token = await GenerateJwtTokenAsync(user);
            return token;
        }

    
        public async Task<string> Register(string username, string password)
        {
            var user = new AplicationUser { UserName = username };
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new Exception("User registration failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            var token = await GenerateJwtTokenAsync(user);
            return token;
        }

       
        private async Task<string> GenerateJwtTokenAsync(AplicationUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
