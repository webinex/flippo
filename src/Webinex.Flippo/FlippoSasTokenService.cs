using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Webinex.Coded;

namespace Webinex.Flippo
{
    internal interface IFlippoSasTokenService
    {
        Task<CodedResult<string>> GetSasTokenAsync([NotNull] IEnumerable<string> references);
        Task<CodedResult> VerifySasTokenAsync(string token, IEnumerable<string> references);
    }

    internal class FlippoSasTokenService : IFlippoSasTokenService
    {
        private static readonly string ISSUER = "flippo";
        private static readonly string REFERENCE_CLAIM = "flippo://reference";

        private readonly IFlippoCoreSettings _settings;
        private Lazy<SymmetricSecurityKey> SecurityKey { get; }
        private Lazy<SigningCredentials> SigningCredentialsLazy { get; }

        public FlippoSasTokenService(IFlippoCoreSettings settings)
        {
            _settings = settings;

            SecurityKey = new Lazy<SymmetricSecurityKey>(() =>
            {
                if (string.IsNullOrWhiteSpace(settings.SasTokenSecret))
                    throw new InvalidOperationException(
                        $"{nameof(settings.SasTokenSecret)} might not be null to use SAS Tokens");

                return Jwt.SymmetricSecurityKey(settings.SasTokenSecret);
            });

            SigningCredentialsLazy = new Lazy<SigningCredentials>(() => Jwt.HmacSha256Signature(SecurityKey.Value));
        }

        public Task<CodedResult<string>> GetSasTokenAsync(IEnumerable<string> references)
        {
            var claims = references.Select(x => new Claim(REFERENCE_CLAIM, x)).ToList();
            var token = Jwt.Encode(
                claims,
                issuer: ISSUER,
                signingCredentials: SigningCredentialsLazy.Value,
                expires: DateTime.UtcNow.Add(_settings.SasTokenTimeToLive!.Value));

            return Task.FromResult(CodedResults.Success(token));
        }

        public async Task<CodedResult> VerifySasTokenAsync(string token, IEnumerable<string> references)
        {
            var result = await Jwt.DecodeAsync(token, SecurityKey.Value, ISSUER);

            if (!result.IsValid)
                return CodedResults.Failed(FlippoCodes.TOKEN);

            var tokenReferences = result.Claims
                .Where(x => x.Key == REFERENCE_CLAIM)
                .Select(x => x.Value.ToString())
                .ToArray();

            if (references.All(x => tokenReferences.Contains(x)))
                return CodedResults.Success();
            
            return CodedResults.Failed(FlippoCodes.TOKEN_REFERENCE);
        }
    }
}