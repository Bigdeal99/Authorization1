using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, user.Role)
            };

            
            var keyString = _config["JWT:Key"] ?? "defaultkeywhichmustbelongerthan64bytestoworkcorrectlywiththehmacshasignaturealgorithm123456!";
            
            
            if (Encoding.UTF8.GetByteCount(keyString) < 64)
            {
                
                keyString = keyString.PadRight(64, '_');
            }
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"] ?? "webapi",
                audience: _config["JWT:Audience"] ?? "webclient",
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}