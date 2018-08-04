using AuthCore.Auth;
using AuthCore.Models;
using AuthCore.Models.Entities;
using AuthCore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;

        public AuthController(UserManager<User> userManager, IJwtFactory jwtFactory, JwtIssuerOptions jwtIssuerOptions)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtIssuerOptions;
        }

        //POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Post([FromBody]CredentialsViewModel credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identity = await GetClaimsIdentity(credentials.UserName, credentials.Password);
            if (identity == null)
            {
                return BadRequest(Helpers.Error.AddErrorToModelState("login failure", "Invalid credentials", ModelState));
            }

            var jwt = await Helpers.Tokens.GenerateJwt
            (
                identity,
                _jwtFactory,
                credentials.UserName,
                _jwtOptions,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                }
            );
            return new OkObjectResult(jwt);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return await Task.FromResult<ClaimsIdentity>(null);
            }

            var userToVerify = await _userManager.FindByNameAsync(username);

            if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);

            if (await _userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(username, userToVerify.Id));
            }
            return await Task.FromResult<ClaimsIdentity>(null);
        }
    }
}