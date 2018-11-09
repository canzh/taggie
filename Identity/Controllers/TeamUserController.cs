using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using taggie.Data;

namespace Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class TeamUserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public IConfiguration _configuration;

        public TeamUserController(UserManager<IdentityUser> userMgr, RoleManager<IdentityRole> roleMgr, IConfiguration configuration)
        {
            _userManager = userMgr;
            _roleManager = roleMgr;
            _configuration = configuration;
        }

        // POST: api/TeamUser
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TeamUserCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            List<TeamUserResponseModel> createdUsers = new List<TeamUserResponseModel>();

            for (int i = 0; i < model.Count; i++)
            {
                string name = $"{model.TeamName}-{i}";

                IdentityUser user = new IdentityUser
                {
                    UserName = name,
                    Email = name + "@sample.com"
                };

                IdentityResult result = await _userManager.CreateAsync(user, _configuration["TempPassword"]);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
                }

                result = await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("team", model.TeamId.ToString()));
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
                }

                var roleName = model.IsQA ? "QA" : "User";

                result = await _userManager.AddToRoleAsync(user, roleName);
                if (!result.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
                }

                createdUsers.Add(new TeamUserResponseModel { UserId = user.Id, UserName = user.UserName });
            }

            return Ok(createdUsers);
        }
    }
}
