using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;

namespace IFormFileConversion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public ValuesController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        // GET api/values
        [HttpGet]
        public ActionResult Get()
        {
            //Test();
            return Ok("Welcome");
        }
        [HttpGet("download")]
        public FileStreamResult Download()
        {
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            var root = Path.Combine(contentRootPath, "Files");
            var filepath = Path.Combine(root, "Startup.cs");
            var memory = new MemoryStream();
            using (var stream = new FileStream(filepath, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(filepath), Path.GetFileName(filepath));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".cs", "text/plain"},
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        [HttpGet("read")]
        public IActionResult ReadIFormFile()
        {
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            var fileLocation = Path.Combine(contentRootPath, "Files");
            var file = Path.Combine(fileLocation, "Startup.cs");
            using (FileStream fileStream = new FileStream(file, FileMode.Open))
            {
                IFormFile fileInfo = new FormFile(fileStream, 0, fileStream.Length, null, Path.GetFileName(fileStream.Name))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = MediaTypeNames.Application.Octet
                };
                var newLocation = Path.Combine(contentRootPath, "New");
                var filePath = Path.Combine(newLocation, $"New{fileInfo.FileName}");
                using (var fileStreamx = new FileStream(filePath, FileMode.Create))
                {
                    fileInfo.CopyTo(fileStreamx);
                }
                fileStream.Close();
            }
            return Ok("Copied Successfully");
        }
    }
}
