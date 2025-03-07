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

            // Get the key from configuration
            var keyString = _config["JWT:Key"] ?? "defaultkeywhichmustbelongerthan64bytestoworkcorrectlywiththehmacshasignaturealgorithm123456!";
            
            // Ensure key is long enough (at least 64 bytes for HMAC-SHA512)
            if (Encoding.UTF8.GetByteCount(keyString) < 64)
            {
                // Pad the key if it's too short
                keyString = keyString.PadRight(64, '_');
            }
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            // Use HMAC-SHA256 instead of SHA512 for more compatibility and less key length requirements
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