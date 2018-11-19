using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Content.Mvc.Models;
using Content.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Content.Mvc.Controllers
{
    [Authorize]
    public class TaggieController : Controller
    {
        private readonly IStringLocalizer<TeamController> _localizer;
        private readonly IDataService _apiService;
        private IHttpContextAccessor _httpAccessor;

        public TaggieController(IStringLocalizer<TeamController> localizer, IDataService apiService, IHttpContextAccessor httpAccessor)
        {
            _localizer = localizer;
            _apiService = apiService;
            _httpAccessor = httpAccessor;
        }

        // GET: Taggie
        public async Task<IActionResult> Index()
        {
            var user = _httpAccessor.HttpContext.User;

            var list = await _apiService.GetTaggieProjectList();

            return View(list);
        }

        // GET: Taggie/Queue/5
        public async Task<IActionResult> Queue(int projectId)
        {
            var categories = await _apiService.GetProjectCategories(projectId);
            var subcategories = await _apiService.GetProjectSubcategories(projectId);


            TaggieQueueViewModel viewModel = new TaggieQueueViewModel();
            viewModel.AllCategories = categories.Select(d => d.Name).ToList();
            viewModel.AllSubcategories = subcategories.Select(d => d.Name).ToList();

            viewModel.ProjectName = "360 Project";
            viewModel.TotalProjectItems = 200_000;
            viewModel.RemainingProjectItems = 100_000;
            viewModel.TaggieFinishedItems = 100;

            return View(viewModel);
        }

    }
}