using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using taggie.Models;

namespace taggie.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<UserListViewModel> model = new List<UserListViewModel>();
            model = userManager.Users.Select(u => new UserListViewModel
            {
                Id = u.Id,
                Name = u.UserName,
                Email = u.Email
            }).ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            UserViewModel model = new UserViewModel();
            model.ApplicationRoles = roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id
            }).ToList();
            return PartialView("_AddUser", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = model.UserName,
                    Email = model.Email
                };
                IdentityResult result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    IdentityRole applicationRole = await roleManager.FindByIdAsync(model.ApplicationRoleId);
                    if (applicationRole != null)
                    {
                        IdentityResult roleResult = await userManager.AddToRoleAsync(user, applicationRole.Name);
                        if (roleResult.Succeeded)
                        {
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            EditUserViewModel model = new EditUserViewModel();
            model.ApplicationRoles = roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id
            }).ToList();

            if (!String.IsNullOrEmpty(id))
            {
                IdentityUser user = await userManager.FindByIdAsync(id);
                var roleName = userManager.GetRolesAsync(user).Result.FirstOrDefault();

                if (user != null)
                {
                    model.Name = user.UserName;
                    model.Email = user.Email;
                }

                if (!string.IsNullOrEmpty(roleName))
                {
                    model.ApplicationRoleId = roleManager.Roles.Single(r => r.Name == roleName).Id;
                }
            }
            return PartialView("_EditUser", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(string id, EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await userManager.FindByIdAsync(id);
                if (user != null)
                {
                    user.UserName = model.Name;
                    user.Email = model.Email;

                    IdentityResult result = await userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        string existingRole = userManager.GetRolesAsync(user).Result.FirstOrDefault();
                        string existingRoleId = existingRole == null ? null : roleManager.Roles.Single(r => r.Name == existingRole).Id;

                        if (existingRoleId != model.ApplicationRoleId)
                        {
                            if (existingRoleId != null)
                            {
                                await userManager.RemoveFromRoleAsync(user, existingRole);
                            }

                            if (model.ApplicationRoleId != null)
                            {
                                IdentityRole applicationRole = await roleManager.FindByIdAsync(model.ApplicationRoleId);
                                if (applicationRole != null)
                                {
                                    await userManager.AddToRoleAsync(user, applicationRole.Name);
                                }
                            }
                        }
                    }
                }
            }

            return RedirectToAction("Index");

            //return PartialView("_EditUser", model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {
            string name = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                IdentityUser applicationUser = await userManager.FindByIdAsync(id);
                if (applicationUser != null)
                {
                    name = applicationUser.UserName;
                }
            }
            return PartialView("_DeleteUser", name);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id, IFormCollection form)
        {
            if (!String.IsNullOrEmpty(id))
            {
                IdentityUser applicationUser = await userManager.FindByIdAsync(id);
                if (applicationUser != null)
                {
                    IdentityResult result = await userManager.DeleteAsync(applicationUser);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View();
        }
    }
}
