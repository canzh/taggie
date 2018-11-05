using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Content.Api.EF;
using Microsoft.AspNetCore.Authorization;

namespace Content.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubcategoriesController : ControllerBase
    {
        private readonly ContentApiContext _context;

        public SubcategoriesController(ContentApiContext context)
        {
            _context = context;
        }

        // GET: api/Subcategories
        [NonAction]
        [HttpGet]
        public IEnumerable<Subcategory> GetSubcategory()
        {
            return _context.Subcategory;
        }

        // GET: api/Subcategories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubcategory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var subcategory = await _context.Subcategory.FindAsync(id);

            if (subcategory == null)
            {
                return NotFound();
            }

            return Ok(subcategory);
        }

        [HttpGet]
        public async Task<IActionResult> GetSubcategory([FromQuery] string query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Subcategory> subcategory;

            if (!string.IsNullOrEmpty(query))
            {
                subcategory = await _context.Subcategory.Where(d => d.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase)).ToListAsync();
            }
            else
            {
                subcategory = await _context.Subcategory.ToListAsync();
            }

            if (subcategory == null)
            {
                return NotFound();
            }

            return Ok(subcategory);
        }

        // PUT: api/Subcategories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubcategory([FromRoute] int id, [FromBody] Subcategory subcategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != subcategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(subcategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubcategoryExists(id))
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

        // POST: api/Subcategories
        [HttpPost]
        public async Task<IActionResult> PostSubcategory([FromBody] Subcategory subcategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Subcategory.Add(subcategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubcategory", new { id = subcategory.Id }, subcategory);
        }

        // DELETE: api/Subcategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubcategory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var subcategory = await _context.Subcategory.FindAsync(id);
            if (subcategory == null)
            {
                return NotFound();
            }

            _context.Subcategory.Remove(subcategory);
            await _context.SaveChangesAsync();

            return Ok(subcategory);
        }

        private bool SubcategoryExists(int id)
        {
            return _context.Subcategory.Any(e => e.Id == id);
        }
    }
}