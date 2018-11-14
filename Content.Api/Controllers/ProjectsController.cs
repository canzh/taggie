using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Content.Api.EFModels;

namespace Content.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public ProjectsController(ApiDbContext context)
        {
            _context = context;
        }

        // GET: api/Projects
        [HttpGet]
        public IEnumerable<Project> GetProject()
        {
            return _context.Project;
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