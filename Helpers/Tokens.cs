using AuthCore.Auth;
using AuthCore.Models;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthCore.Helpers
{
    public class Tokens
    {
        public static async Task<string> GenerateJwt(ClaimsIdentity identity, IJwtFactory jwtFactory, string username, JwtIssuerOptions jwtIssuerOptions, JsonSerializerSettings serializerSettings)
        {
            var response = new
            {
                id = identity.Claims.Single(c => c.Type == "id").Value,
                auth_token = await jwtFactory.GenerateEncodedToken(username, identity),
                expires_in = (int)jwtIssuerOptions.ValidFor.TotalSeconds
            };

            return JsonConvert.SerializeObject(response, serializerSettings);
        }
    }
}
