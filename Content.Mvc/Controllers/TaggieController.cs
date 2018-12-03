using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Content.Mvc.Models;
using Content.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Content.Mvc.Controllers
{
    [Authorize(Policy = "HasTeam")]
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
        public async Task<IActionResult> Queue([FromRoute] int id)
        {
            var viewModel = await _apiService.GetNextQueueItem(id);

            if (viewModel == null)
            {
                return View(null);
            }

            var categories = await _apiService.GetProjectCategories(id);
            var subcategories = await _apiService.GetProjectSubcategories(id);

            viewModel.AllCategories = categories.Select(d => d.Name).ToList();
            viewModel.AllSubcategories = subcategories.Select(d => d.Name).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> QueueItemContent([FromRoute] int id)
        {
            var content = await _apiService.GetQueueItemContent(id);
            return Ok(HttpUtility.HtmlEncode(content));
        }

        public async Task<IActionResult> Submit(TaggieQueueSubmitModel model)
        {
            await _apiService.SubmitQueueItem(model);

            return Ok();
        }
    }
}