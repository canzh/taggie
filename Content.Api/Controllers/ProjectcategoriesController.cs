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
    public class ProjectcategoriesController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public ProjectcategoriesController(ApiDbContext context)
        {
            _context = context;
        }

        // GET: api/Projectcategories
        [HttpGet]
        public IEnumerable<Projectcategory> GetProjectcategory()
        {
            return _context.Projectcategory;
        }

        // GET: api/Projectcategories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectcategory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var projectcategory = await _context.Projectcategory.FindAsync(id);

            if (projectcategory == null)
            {
                return NotFound();
            }

            return Ok(projectcategory);
        }

        // PUT: api/Projectcategories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectcategory([FromRoute] int id, [FromBody] Projectcategory projectcategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != projectcategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(projectcategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectcategoryExists(id))
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

        // POST: api/Projectcategories
        [HttpPost]
        public async Task<IActionResult> PostProjectcategory([FromBody] Projectcategory projectcategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Projectcategory.Add(projectcategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProjectcategory", new { id = projectcategory.Id }, projectcategory);
        }

        // DELETE: api/Projectcategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectcategory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var projectcategory = await _context.Projectcategory.FindAsync(id);
            if (projectcategory == null)
            {
                return NotFound();
            }

            _context.Projectcategory.Remove(projectcategory);
            await _context.SaveChangesAsync();

            return Ok(projectcategory);
        }

        private bool ProjectcategoryExists(int id)
        {
            return _context.Projectcategory.Any(e => e.Id == id);
        }
    }
}