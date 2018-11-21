using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Content.Api.EFModels;
using Content.Api.EFModels.enums;
using Content.Api.EFModels.dto;
using Content.Api.Common;
using Content.Api.Event;

namespace Content.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectitemsController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserAccessValidation _accessValidation;
        private readonly RedisUtil _redis;

        public ProjectitemsController(ApiDbContext context,
            IHttpContextAccessor httpContext,
            UserAccessValidation accessValidation,
            RedisUtil redis)
        {
            _context = context;
            _httpContext = httpContext;
            _accessValidation = accessValidation;
            _redis = redis;
        }

        [HttpGet("{projectId}")]
        [Route("next")]
        public async Task<IActionResult> GetNextQueueItem([FromQuery] int projectId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var projectitem = await _context.Projectitem.Include(e => e.Project).FirstOrDefaultAsync(
            //    d => d.Status == ProjectItemStatus.New);

            var assignedItemId = await _redis.AssignQueueItemToUser(projectId, _accessValidation.GetUserSubId());

            if (assignedItemId == 0)
            {
                return NoContent();
            }

            var pojectInfo = await _redis.GetProjectInfo(projectId);

            QueueItemDto result = new QueueItemDto
            {
                ProjectId = projectId,
                ProjectName = pojectInfo.ProjectName,
                TotalProjectItems = pojectInfo.TotalItems,
                RemainingProjectItems = pojectInfo.RemainingItems,
                ProjectItemId = assignedItemId,
            };

            return Ok(result);
        }

        // GET: api/Projectitems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectitem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var projectitem = await _context.Projectitem.FindAsync(id);

            if (projectitem == null)
            {
                return NotFound();
            }

            return Ok(projectitem);
        }

        // POST: api/Projectitems
        [HttpPost]
        [Route("submit")]
        public async Task<IActionResult> SubmitQueueItem([FromBody] QueueItemSubmitDto projectitem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = await _context.Projectitem.FindAsync(projectitem.ProjectItemId);
            if (item == null)
            {
                return BadRequest($"ProjectItem does not exist with Id: {projectitem.ProjectItemId}");
            }

            if (!await _accessValidation.IfQueueItemAssignedToUser(item.ProjectId, projectitem.ProjectItemId))
            {
                return BadRequest($"Forbidden to edit item which not assigned to you: {projectitem.ProjectItemId}");
            }

            var cIds = await _redis.ConvertCategoryNames(item.ProjectId, projectitem.CategoryNames);
            var scIds = await _redis.ConvertSubcategoryNames(item.ProjectId, projectitem.SubcategoryNames);
            var userId = _accessValidation.GetUserSubId();

            var tevent = new TaggedEvent
            {
                TeamId = _accessValidation.GetUserTeamId(),
                ProjectId = item.ProjectId,
                ProjectItemId = projectitem.ProjectItemId,
                TaggedUserId = userId,
                CategoryIds = cIds,
                SubcategoryIds = scIds,
                Keywords = projectitem.Keywords
            };

            await _redis.PublishTaggedItem(tevent);
            await _redis.SetUserSubmitted(item.ProjectId, userId, true);

            return Ok();
        }
    }
}