using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Content.Mvc.Models;
using Content.Mvc.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Content.Mvc.Controllers
{
    public class TeamController : Controller
    {
        // http://localhost:7004/?culture=es-MX&ui-culture=es-MX
        private readonly IStringLocalizer<TeamController> _localizer;
        private readonly IDataService _apiService;
        private readonly IIdentityService _identityService;

        public TeamController(IStringLocalizer<TeamController> localizer, IDataService apiService, IIdentityService identityService)
        {
            _localizer = localizer;
            _apiService = apiService;
            _identityService = identityService;
        }

        // GET: Team
        public async Task<IActionResult> Index()
        {
            var list = await _apiService.GetAllTeams();
            return View(list);
        }

        // GET: Team/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var team = await _apiService.GetTeamDetail(id);

            //var assignments = await _apiService.GetTeamAssignments(id);
            //team.AssignedProjects = assignments;

            var members = await _identityService.GetTeamMembers(id);
            team.TeamMembers = members.ToList();

            return View(team);
        }

        // GET: Team/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Team/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeamViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newId = await _apiService.CreateNewTeam(model);

                    await _identityService.CreateTeamUsers(
                        new Services.DTO.IdentityRequestModel
                        {
                            TeamId = newId,
                            MemberCount = model.MemberCount,
                            TeamName = model.TeamName,
                            IsQA = model.IsQATeam
                        }
                    );

                    return RedirectToAction(nameof(Details), new { id = newId });
                }
            }
            catch
            {
                return View();
            }

            return View();
        }

        // GET: Team/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Team/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Team/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Team/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}