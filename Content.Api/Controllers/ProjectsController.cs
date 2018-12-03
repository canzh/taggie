using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Content.Api.EFModels;
using Content.Api.EFModels.dto;
using Microsoft.AspNetCore.Authorization;

namespace Content.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public ProjectsController(ApiDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        // GET: api/Projects
        [HttpGet]
        [Authorize(Policy = "Admin")]
        public IEnumerable<Project> GetProject()
        {
            return _context.Project;
        }

        // GET: api/Projects/taggie
        [HttpGet]
        [Route("taggie")]
        [Authorize(Policy = "HasTeam")]
        public async Task<IActionResult> GetTaggieProjectList()
        {
            var loginUser = _httpContext.HttpContext.User;
            var teamIdValue = loginUser.Claims.FirstOrDefault(d => d.Type == "team")?.Value;

            if (string.IsNullOrEmpty(teamIdValue))
            {
                BadRequest("Can not find team information for the login user!");
            }

            int teamId;

            if (!int.TryParse(teamIdValue, out teamId))
            {
                BadRequest("Can not find team information for the login user!");
            }

            var projects = _context.Project.Include(e => e.Teamprojects).Where(d => d.Teamprojects.Any(r => r.TeamId == teamId));

            List<TaggieProject> result = new List<TaggieProject>();
            foreach (var p in projects)
            {
                result.Add(new TaggieProject
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    Description = p.Description,
                    AssginedItemsCount = p.Teamprojects.FirstOrDefault(d => d.TeamId == teamId)?.AssignedProjectItems ?? 0
                });
            }

            return Ok(result);
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = await _context.Project
                .Include(e => e.Teamprojects)
                .ThenInclude(e => e.Team)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            var result = new ProjectDto
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                Description = project.Description,
                BaseDir = project.BaseDir,
                Status = project.Status,
                TotalItems = project.TotalItems,
                CreatedDate = project.CreatedDate,
                Assignments = project.Teamprojects.Select(d =>
                    new ProjectAssignment
                    {
                        TeamId = d.TeamId,
                        TeamName = d.Team.TeamName,
                        AssignedItemsCount = d.AssignedProjectItems
                    }).ToList()
            };

            return Ok(result);
        }

        // PUT: api/Projects/5
        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> PutProject([FromRoute] int id, [FromBody] Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != project.Id)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Projects
        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> PostProject([FromBody] Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Project.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.Id }, project);
        }

        // POST: api/Projects/1
        [HttpPost("{projectId}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> PostAssignItemsToTeam(int projectId, [FromBody] ProjectAssignment assignment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = await _context.Project.FindAsync(projectId);

            if (project == null)
            {
                return NotFound();
            }

            var team = await _context.Team.FindAsync(assignment.TeamId);

            if (team == null)
            {
                return NotFound();
            }

            if (await _context.Teamprojects.FirstOrDefaultAsync(d => d.TeamId == team.Id && d.ProjectId == project.Id) != null)
            {
                return BadRequest($"Team {team.TeamName} is already linked with Project {project.ProjectName}");
            }

            project.Teamprojects.Add(new Teamprojects { Project = project, Team = team, AssignedProjectItems = assignment.AssignedItemsCount });

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteProject([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Project.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(project);
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.Id == id);
        }
    }
}