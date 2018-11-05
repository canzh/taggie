using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Content.Api.EF;
using Content.Api.Models;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace Content.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContentFilePathController : ControllerBase
    {
        private readonly ContentApiContext _context;

        public ContentFilePathController(ContentApiContext context)
        {
            _context = context;
        }

        // GET: api/ContentFilePath
        [HttpGet]
        [ProducesResponseType(typeof(ContentFilePathTreeModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetContentfilepath()
        {
            var root = await _context.Contentfilepath.FirstOrDefaultAsync(d => d.Name == "Root");

            if (root == null)
            {
                return NoContent();
            }


            ContentFilePathTreeModel result = new ContentFilePathTreeModel();
            result.success = true;
            result.children = BuildWholeTree(root);

            return Ok(result);
        }

        ContentFilePathDto BuildWholeTree(Contentfilepath root)
        {
            var children = _context.Contentfilepath.Where(d => d.ParentId == root.Id).ToList();

            if (children.Count == 0)
            {
                var result = new ContentFilePathDto
                {
                    id = root.Id,
                    name = root.Name,
                    path = root.FullPath,
                    loaded = true
                };

                return result;
            }

            var end = new ContentFilePathDto
            {
                id = root.Id,
                name = root.Name,
                path = root.FullPath,
                loaded = true,
                expanded = true
            };

            foreach(var child in children)
            {
                var temp = BuildWholeTree(child);

                end.children.Add(temp);
            }

            return end;
        }
    }
}