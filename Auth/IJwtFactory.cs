using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthCore.Auth
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string username, ClaimsIdentity identity);
        ClaimsIdentity GenerateClaimsIdentity(string username, string id);
    }
}
