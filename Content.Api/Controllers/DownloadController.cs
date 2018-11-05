using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Content.Api.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace Content.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class DownloadController : ControllerBase
    {
        private readonly ContentApiContext _context;
        private readonly IHostingEnvironment _env;

        public DownloadController(IHostingEnvironment env, ContentApiContext context)
        {
            _env = env;
            _context = context;
        }

        // GET: api/ContentFile/5
        [HttpGet("{contentFileId}")]
        public async Task<IActionResult> GetFile([FromRoute]int contentFileId)
        {
            if (contentFileId <= 0)
            {
                return BadRequest();
            }

            var item = await _context.Contentfile.Include(e => e.FilePath).FirstOrDefaultAsync(ci => ci.Id == contentFileId);

            if (item != null)
            {
                var webRoot = _env.WebRootPath;
                var path = Path.Combine(webRoot, item.FilePath.FullPath, item.Name);

                var provider = new FileExtensionContentTypeProvider();
                string contentType;
                if (!provider.TryGetContentType(path, out contentType))
                {
                    contentType = "application/octet-stream";
                }

                var buffer = System.IO.File.ReadAllBytes(path);

                return File(buffer, contentType);
            }

            return NotFound();
        }
    }
}