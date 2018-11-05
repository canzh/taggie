using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Content.Api.EF;
using Content.Web.Models;
using Content.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace Content.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContentFileController : ControllerBase
    {
        private readonly ContentApiContext _context;

        public ContentFileController(ContentApiContext context)
        {
            _context = context;
        }

        // GET: api/ContentFile
        [NonAction]
        [HttpGet]
        public IEnumerable<ContentFileDto> GetContentfile()
        {
            var list = _context.Contentfile
                .Include(e => e.Filecategory)
                .ThenInclude(e => e.Category).ToList();

            var result = list.Select(s =>
                new ContentFileDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    CreatedDate = s.CreatedDate,
                    FilePathId = s.FilePathId,
                    Status = s.Status,
                    FinishedBy = s.FinishedBy,
                    VerifiedBy = s.VerifiedBy,
                    Categories = s.Filecategory.Select(o => o.Category.Name).ToList(),
                    Subcategories = s.Filesubcategory.Select(o => o.SubCategory.Name).ToList(),
                    FinishedDate = s.FinishedDate,
                    VerifiedDate = s.VerifiedDate
                }
            );

            return result;
        }

        // GET: api/ContentFile/5
        [NonAction]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContentfile([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var s = await _context.Contentfile
                .Include(e => e.Filecategory)
                .ThenInclude(e => e.Category)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (s == null)
            {
                return NotFound();
            }

            var result = new ContentFileDto
            {
                Id = s.Id,
                Name = s.Name,
                CreatedDate = s.CreatedDate,
                FilePathId = s.FilePathId,
                Status = s.Status,
                FinishedBy = s.FinishedBy,
                VerifiedBy = s.VerifiedBy,
                Categories = s.Filecategory.Select(o => o.Category.Name).ToList(),
                Subcategories = s.Filesubcategory.Select(o => o.SubCategory.Name).ToList(),
                FinishedDate = s.FinishedDate,
                VerifiedDate = s.VerifiedDate
            };

            return Ok(result);
        }

        // GET: api/ContentFile?filePathId=5&status=new&start=0&limit=20
        [HttpGet]
        //[Route("filter")]
        public async Task<IActionResult> GetContentfile([FromQuery] int filePathId, [FromQuery] string status, [FromQuery] int? start, [FromQuery] int? limit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ss = (SourceStatus)Enum.Parse(typeof(SourceStatus), status ?? "New");

            var total = _context.Contentfile
                .Where<Contentfile>(c => c.FilePathId == filePathId && c.Status == ss)
                .Count();

            var contentfileList = _context.Contentfile
                .Include(e => e.Filecategory).ThenInclude(e => e.Category)
                .Include(e => e.Filesubcategory).ThenInclude(e => e.SubCategory)
                .Where<Contentfile>(c => c.FilePathId == filePathId && c.Status == ss)
                .Skip(start ?? 0)
                .Take(limit ?? 20)
                .ToList();

            List<ContentFileDto> result = new List<ContentFileDto>();

            foreach (var s in contentfileList)
            {
                result.Add(new ContentFileDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    CreatedDate = s.CreatedDate,
                    FilePathId = s.FilePathId,
                    Status = s.Status,
                    FinishedBy = s.FinishedBy,
                    VerifiedBy = s.VerifiedBy,
                    Categories = s.Filecategory.Select(o => o.Category.Name).ToList(),
                    Subcategories = s.Filesubcategory.Select(o => o.SubCategory.Name).ToList(),
                    FinishedDate = s.FinishedDate,
                    VerifiedDate = s.VerifiedDate
                });
            }

            return Ok(new
            {
                data = result,
                total = total
            });
        }

        // PUT: api/ContentFile/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContentfile([FromRoute] int id, [FromBody] ContentFileDto contentfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contentfile.Id)
            {
                return BadRequest();
            }

            var contentFile = await _context.Contentfile
                .Include(e => e.Filecategory).ThenInclude(e => e.Category)
                .Include(e => e.Filesubcategory).ThenInclude(e => e.SubCategory)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (contentFile == null)
            {
                return NotFound();
            }

            // For Category
            var oldArray = contentFile.Filecategory.Select(o => o.Category.Name).ToList();
            oldArray.Sort();
            string oldKwds = string.Join(";", oldArray);
            contentfile.Categories.Sort();
            string newKwds = string.Join(";", contentfile.Categories);

            if (newKwds != oldKwds)
            {
                // Update data
                contentFile.Filecategory.Clear();
                foreach (var keywd in contentfile.Categories)
                {
                    var ck = await _context.Category.FirstOrDefaultAsync(k => k.Name.ToLower() == keywd.ToLower());

                    if (ck == null)
                    {
                        throw new Exception("Category not found: " + keywd);
                    }

                    contentFile.Filecategory.Add(new Filecategory { File = contentFile, Category = ck });
                }
            }

            // For SubCategory
            var oldSub = contentFile.Filesubcategory.Select(o => o.SubCategory.Name).ToList();
            oldSub.Sort();
            string oldSubj = string.Join(";", oldSub);
            contentfile.Subcategories.Sort();
            string newSubj = string.Join(";", contentfile.Subcategories);

            if (oldSubj != newSubj)
            {
                // Update data
                contentFile.Filesubcategory.Clear();
                foreach (var subj in contentfile.Subcategories)
                {
                    var ck = await _context.Subcategory.FirstOrDefaultAsync(k => k.Name.ToLower() == subj.ToLower());

                    if (ck == null)
                    {
                        throw new Exception("Subcategory not found: " + subj);
                    }

                    contentFile.Filesubcategory.Add(new FileSubcategory { File = contentFile, SubCategory = ck });
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContentfileExists(id))
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

        [HttpPut]
        [Route("action")]
        public async Task<IActionResult> PutContentfile([FromForm] string ids, [FromForm] string actionType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            switch (actionType)
            {
                case "Archive":
                    {
                        return await ArchiveAction(ids);
                    }
                default:
                    return BadRequest("action type was not recgonized!");
            }
        }

        [NonAction]
        async Task<IActionResult> ArchiveAction(string ids)
        {
            //if (!User.IsInRole("Admin"))
            //{
            //    return Forbid();
            //}

            string[] idArray = ids.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var idStr in idArray)
            {
                int webId = int.Parse(idStr);
                var dbRecord = await _context.Contentfile.FirstOrDefaultAsync(d => d.Id == webId);

                dbRecord.Status = SourceStatus.Archived;
            }

            _context.SaveChanges();

            return NoContent();
        }


        private bool ContentfileExists(int id)
        {
            return _context.Contentfile.Any(e => e.Id == id);
        }
    }
}