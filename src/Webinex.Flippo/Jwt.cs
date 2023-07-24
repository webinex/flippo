using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Webinex.Flippo
{
    internal static class Jwt
    {
        public static SigningCredentials HmacSha256Signature(SecurityKey securityKey)
        {
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        }

        public static SymmetricSecurityKey SymmetricSecurityKey(string secret)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        }

        public static string Encode(
            IEnumerable<Claim> claims,
            string issuer,
            SigningCredentials signingCredentials,
            DateTime expires)
        {
            var token = new JwtSecurityToken(
                claims: claims,
                expires: expires,
                signingCredentials: signingCredentials,
                issuer: issuer);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static async Task<TokenValidationResult> DecodeAsync(
            string token,
            SecurityKey securityKey,
            string issuer)
        {
            return await new JwtSecurityTokenHandler().ValidateTokenAsync(token, new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidIssuer = issuer,
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ClockSkew = TimeSpan.Zero,
            });
        }
    }
}