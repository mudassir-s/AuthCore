using AuthCore.Models;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace AuthCore.Auth
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtIssuerOptions _jwtIssuerOptions;

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() -
                new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
            .TotalSeconds);

        public JwtFactory(IOptions<JwtIssuerOptions> options)
        {
            _jwtIssuerOptions = options.Value;
            ThrowIfInvalidOptions(_jwtIssuerOptions);
        }

        private void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if(options.ValidFor <=TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero timespan", nameof(JwtIssuerOptions.ValidFor));
            }

            if(options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if(options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        public ClaimsIdentity GenerateClaimsIdentity(string username, string id)
        {
            return new ClaimsIdentity(new GenericIdentity(username, "Token"), new[]
            {
                new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Id, id),
                new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Rol, Helpers.Constants.Strings.JwtClaims.ApiAccess)
            });
        }

        public async Task<string> GenerateEncodedToken(string username, ClaimsIdentity identity)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtIssuerOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtIssuerOptions.IssuedAt).ToString(),ClaimValueTypes.Integer64),
                identity.FindFirst(Helpers.Constants.Strings.JwtClaimIdentifiers.Rol),
                identity.FindFirst(Helpers.Constants.Strings.JwtClaimIdentifiers.Id)
            };

            var jwt = new JwtSecurityToken
                (
                issuer: _jwtIssuerOptions.Issuer,
                audience: _jwtIssuerOptions.Audience,
                claims: claims,
                notBefore: _jwtIssuerOptions.NotBefore,
                expires: _jwtIssuerOptions.Expiration,
                signingCredentials: _jwtIssuerOptions.SigningCredentials
                );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }
    }
}
