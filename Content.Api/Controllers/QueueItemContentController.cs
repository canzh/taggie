using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Content.Api.EFModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Content.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class QueueItemContentController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IHostingEnvironment _env;

        public QueueItemContentController(ApiDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("{projectItemId}")]
        public async Task<IActionResult> GetProjectItemContent([FromRoute] int projectItemId)
        {
            var projectitem = await _context.Projectitem.Include(e => e.Project).FirstOrDefaultAsync(d => d.Id == projectItemId);

            if (projectitem == null)
            {
                return NotFound();
            }

            var webRoot = _env.WebRootPath;
            var path = Path.Combine(webRoot, projectitem.Project.BaseDir, projectitem.RelativeDir, projectitem.LocalFileName);

            var buffer = System.IO.File.ReadAllBytes(path);
            return File(buffer, "text/plain; charset=utf-8");
        }
    }
}