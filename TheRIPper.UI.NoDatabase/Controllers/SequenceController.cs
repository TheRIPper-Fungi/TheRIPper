using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bio.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using TheRIPper.UI.NoDatabase.Models;

namespace TheRIPper.UI.NoDatabase.Controllers
{
    public class SequenceController : Controller
    {
        private IMemoryCache _cache;

        public SequenceController(IMemoryCache cache) {
            _cache = cache;
        }
        public IActionResult Sequences() {
            return View();
        }

        [HttpGet]
        [Route("api/sequence/{FileName}")]
        public JsonResult GetSequencesByFileId(string FileName) {

            var sequences = SessionManagement
                .SessionMethods.Get<FileModels>(HttpContext.Session, FileName, false, _cache)
                .Sequences.Select(s=>new {
                    Id = s.Id,
                    Name = s.SequenceName,
                    Length = s.SequenceContent.Length,
                    GCContent = s.GCContent
                }).ToList();

            return new JsonResult(JsonConvert.SerializeObject(sequences)) { ContentType = "application/json", StatusCode = 200 };
        }
    }
}