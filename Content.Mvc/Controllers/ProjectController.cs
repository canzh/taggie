using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Content.Mvc.Models;
using Content.Mvc.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace Content.Mvc.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IStringLocalizer<TeamController> _localizer;
        private readonly IDataService _apiService;
        private readonly IIdentityService _identityService;

        public ProjectController(IStringLocalizer<TeamController> localizer, IDataService apiService, IIdentityService identityService)
        {
            _localizer = localizer;
            _apiService = apiService;
            _identityService = identityService;
        }

        // GET: Project
        public async Task<IActionResult> Index()
        {
            var list = await _apiService.GetAllProjects();
            return View(list);
        }

        // GET: Project/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var project = await _apiService.GetProjectDetail(id);
            return View(project);
        }

        [HttpGet]
        public async Task<IActionResult> AssignToTeam(int id)
        {
            var allTeams = await _apiService.GetAllTeams();

            ProjectAssignmentViewModel model = new ProjectAssignmentViewModel();
            model.Teams = allTeams
                .Where(d => d.Status == TeamStatus.Active)
                .Select(r => new SelectListItem
                {
                    Text = r.TeamName,
                    Value = r.Id.ToString()
                }).ToList();

            return PartialView("_AssignToTeam", model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignToTeam(int id, ProjectAssignmentViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return PartialView("_AssignToTeam", model);
            //}

            var teamId = model.TeamId;

            if (teamId > 0)
            {
                await _apiService.AssignProjectItemsToTeam(id, model);
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }
    }
}