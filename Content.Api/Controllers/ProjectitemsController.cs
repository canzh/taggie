using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Content.Api.EFModels;
using Content.Api.EFModels.enums;

namespace Content.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectitemsController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public ProjectitemsController(ApiDbContext context)
        {
            _context = context;
        }

        // GET: api/Projectitems
        [HttpGet]
        public IEnumerable<Projectitem> GetProjectitem()
        {
            return _context.Projectitem;
        }

        [HttpGet("{projectId}")]
        [Route("next")]
        public async Task<IActionResult> GetNextQueueItem([FromQuery] int projectId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var projectitem = await _context.Projectitem.FirstOrDefaultAsync(
                d => d.Status == ProjectItemStatus.New);

            if (projectitem == null)
            {
                return NotFound();
            }

            return Ok(projectitem);
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

        // PUT: api/Projectitems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectitem([FromRoute] int id, [FromBody] Projectitem projectitem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != projectitem.Id)
            {
                return BadRequest();
            }

            _context.Entry(projectitem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectitemExists(id))
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

        // POST: api/Projectitems
        [HttpPost]
        public async Task<IActionResult> PostProjectitem([FromBody] Projectitem projectitem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Projectitem.Add(projectitem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProjectitem", new { id = projectitem.Id }, projectitem);
        }

        // DELETE: api/Projectitems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectitem([FromRoute] int id)
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

            _context.Projectitem.Remove(projectitem);
            await _context.SaveChangesAsync();

            return Ok(projectitem);
        }

        private bool ProjectitemExists(int id)
        {
            return _context.Projectitem.Any(e => e.Id == id);
        }
    }
}