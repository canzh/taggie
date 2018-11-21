using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Content.Api.EFModels;
using Content.Api.Common;

namespace Content.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsubcategoriesController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly RedisUtil _redis;

        public ProjectsubcategoriesController(ApiDbContext context, RedisUtil redis)
        {
            _context = context;
            _redis = redis;
        }

        // GET: api/Projectsubcategories?projectId=1
        [HttpGet]
        public async Task<IActionResult> GetProjectsubcategory([FromQuery] int projectId)
        {
            var subcategories = await _redis.GetProjectSubcategories(projectId);
            var model = subcategories.Select(d => new { Name = d }).ToList();
            return Ok(model);
        }

        // PUT: api/Projectsubcategories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectsubcategory([FromRoute] int id, [FromBody] Projectsubcategory projectsubcategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != projectsubcategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(projectsubcategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectsubcategoryExists(id))
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

        // POST: api/Projectsubcategories
        [HttpPost]
        public async Task<IActionResult> PostProjectsubcategory([FromBody] Projectsubcategory projectsubcategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Projectsubcategory.Add(projectsubcategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProjectsubcategory", new { id = projectsubcategory.Id }, projectsubcategory);
        }

        // DELETE: api/Projectsubcategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectsubcategory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var projectsubcategory = await _context.Projectsubcategory.FindAsync(id);
            if (projectsubcategory == null)
            {
                return NotFound();
            }

            _context.Projectsubcategory.Remove(projectsubcategory);
            await _context.SaveChangesAsync();

            return Ok(projectsubcategory);
        }

        private bool ProjectsubcategoryExists(int id)
        {
            return _context.Projectsubcategory.Any(e => e.Id == id);
        }
    }
}