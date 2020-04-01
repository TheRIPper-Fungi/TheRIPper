using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using TheRIPper.BL.GCContent;
using TheRIPper.BL.SequenceHelpers;
using TheRIPper.UI.NoDatabase.Models;

namespace TheRIPper.UI.NoDatabase.Controllers
{
    public class GCContentController : Controller
    {
        private IMemoryCache _cache;
        public GCContentController(IMemoryCache cache) {
            _cache = cache;
        }
        [HttpGet]
        [Route("api/gccontent/sequence/{FileName}/{SequenceName}")]
        public JsonResult GCContentSingleSequenceTotal(string FileName, string SequenceName) {

            var sequenceObject = SessionManagement.SessionMethods.Get<FileModels>(HttpContext.Session, FileName, false, _cache)
                .Sequences.Where(w => w.SequenceName == SequenceName).FirstOrDefault();

            ISequence sequence = SequenceHelpers.BuildSequenceFromString(sequenceObject.SequenceName, sequenceObject.SequenceContent);


            double GCContent = Math.Round(GCContentLogic.GCContentSingleSequenceTotal(sequence),2);
            return new JsonResult(JsonConvert.SerializeObject(GCContent)) { ContentType = "application/json", StatusCode = 200 };
        }

        [HttpGet]
        [Route("api/gccontent/file/{FileName}")]
        public JsonResult GCContentFileTotal(string FileName) {

            List<ISequence> sequences = new List<ISequence>();
                
            SessionManagement
                .SessionMethods
                .Get<FileModels>(HttpContext.Session, FileName, false, _cache)
                .Sequences
                .ForEach(f => {
                    sequences.Add(SequenceHelpers.BuildSequenceFromString(f.SequenceName, f.SequenceContent));
                });

            ISequence mergedSequence = SequenceHelpers.MergeSequences(sequences);

            double GCContent = Math.Round(GCContentLogic.GCContentSingleSequenceTotal(mergedSequence),2);

            return new JsonResult(JsonConvert.SerializeObject(GCContent)) { ContentType = "application/json", StatusCode = 200 };
        }
    }
}