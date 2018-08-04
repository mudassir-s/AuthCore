using System.Threading.Tasks;
using AuthCore.Data;
using AuthCore.Helpers;
using AuthCore.Models.Entities;
using AuthCore.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthCore.Controllers
{
    /// <summary>
    /// Manages user accounts
    /// </summary>
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private ApplicationDbContext _dbContext;

        public AccountController(UserManager<User> userManager, IMapper mapper, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Add user to the system
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/account
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdentity = _mapper.Map<User>(model);

            var result = await _userManager.CreateAsync(userIdentity, model.Password);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(Error.AddErrorsToModelState(result, ModelState));
            }

            await _dbContext.Customers.AddAsync(new Customer
            {
                IdentityId = userIdentity.Id,
                Location = model.Location
            });

            await _dbContext.SaveChangesAsync();

            return new OkObjectResult("Account created");
        }
    }
}