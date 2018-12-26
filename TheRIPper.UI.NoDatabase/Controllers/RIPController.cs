using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bio;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TheRIPper.BL.Models;
using TheRIPper.BL.RIP;
using TheRIPper.BL.SequenceHelpers;
using TheRIPper.UI.NoDatabase.Models;

namespace TheRIPper.UI.NoDatabase.Controllers
{
    public class RIPController : Controller
    {
        public IActionResult RIPSequenceView() {
            return View();
        }

        /// <summary>
        /// LRAR (Large RIP Affected Regions) View
        /// </summary>
        /// <returns>View</returns>
        public IActionResult LRARView() {
            return View();
        }

        public IActionResult RIPProfileView()
        {
            return View();
        }



        #region APIMethods
        [HttpGet]
        [Route("api/rip/sequence/{FileName}/{SequenceName}/{WindowSize?}/{SlidingSize?}")]
        public JsonResult Test(string FileName, string SequenceName, int? WindowSize, int? SlidingSize) {
            if (WindowSize == null) { WindowSize = 1000; };
            if (SlidingSize == null) { SlidingSize = 500; };

            var sequenceObject = SessionManagement
                .SessionMethods.Get<FileModels>(HttpContext.Session, FileName)
                .Sequences.Where(w => w.SequenceName == SequenceName)
                .FirstOrDefault();
            ISequence sequence = SequenceHelpers.BuildSequenceFromString(sequenceObject.SequenceName, sequenceObject.SequenceContent);

            List<RIPModels> ripModels = RIPLogic.RIPSplitAndSequence(sequence, (int)WindowSize, (int)SlidingSize);

            return new JsonResult(JsonConvert.SerializeObject(ripModels)) { ContentType = "application/json", StatusCode = 200 };
        }

        [HttpGet]
        [Route("api/rip/file/{FileName}/{WindowSize?}/{SlidingSize?}")]
        public JsonResult RIPFile(string FileName, int? WindowSize, int? SlidingSize) {
            if (WindowSize == null) { WindowSize = 1000; };
            if (SlidingSize == null) { SlidingSize = 500; };

            List<SequenceModels> sequenceObjects = SessionManagement
                .SessionMethods
                .Get<FileModels>(HttpContext.Session, FileName)
                .Sequences;
            List<ISequence> sequences = new List<ISequence>();

            sequenceObjects.ForEach(f => {
                sequences.Add(SequenceHelpers.BuildSequenceFromString(f.SequenceName, f.SequenceContent));
            });

            List<RIPModels> RIPdata = RIPLogic.RIPGenome(sequences, (int)WindowSize, (int)SlidingSize);

            return new JsonResult(JsonConvert.SerializeObject(RIPdata)) { ContentType = "application/json", StatusCode = 200 };
        }


        [HttpGet]
        [Route("api/rip/lrar/sequence/{FileName}/{SequenceName}/{window}/{slide}/{compositeRequirement}/{compositeCountRequirement}")]
        public JsonResult LRARSequence(string FileName,string SequenceName, int window, int slide, double compositeRequirement, int compositeCountRequirement) {

            ISequence sequence = SessionManagement
                .SessionMethods
                .Get<FileModels>(HttpContext.Session, FileName)
                .Sequences
                .Where(w => w.SequenceName == SequenceName)
                .Select(s => SequenceHelpers.BuildSequenceFromString(s.SequenceName, s.SequenceContent))
                .FirstOrDefault();

            List<LRARModels> LRARs = LRARLogic.LRARSequence(sequence, window, slide, compositeRequirement, compositeCountRequirement);

            return new JsonResult(JsonConvert.SerializeObject(LRARs)) { ContentType = "application/json", StatusCode = 200 };
        }

        [HttpGet]
        [Route("api/rip/lrar/file/{FileName}/{window}/{slide}/{compositeRequirement}/{compositeCountRequirement}")]
        public JsonResult LRARFile(string FileName, int window, int slide, double compositeRequirement, int compositeCountRequirement) {
            List<ISequence> sequences = SessionManagement
                .SessionMethods
                .Get<FileModels>(HttpContext.Session, FileName)
                .Sequences
                .Select(s => SequenceHelpers.BuildSequenceFromString(s.SequenceName, s.SequenceContent))
                .ToList();

            List<LRARModels> LRARs = new List<LRARModels>();

            Enumerable.Range(0, sequences.Count).AsParallel().ForAll(f => {
                ISequence sequence = sequences[f];
                LRARs.AddRange(LRARLogic.LRARSequence(sequence, window, slide, compositeRequirement, compositeCountRequirement));
            });

            return new JsonResult(JsonConvert.SerializeObject(LRARs)) { ContentType = "application/json", StatusCode = 200 };
        }

        [HttpGet]
        [Route("api/rip/profile/file/{FileName}/{window}/{slide}/{compositeRequirement}/{compositeCountRequirement}")]
        public JsonResult RIPProfileFile(string FileName, int window, int slide, double compositeRequirement, int compositeCountRequirement) {

            List<ISequence> sequences = SessionManagement
                .SessionMethods
                .Get<FileModels>(HttpContext.Session, FileName)
                .Sequences
                .Select(s => SequenceHelpers.BuildSequenceFromString(s.SequenceName, s.SequenceContent))
                .ToList();

            var results = RIPProfileLogic.RIPFileProfile(sequences, window, slide, compositeRequirement, compositeCountRequirement, FileName);

            return new JsonResult(JsonConvert.SerializeObject(results)) { ContentType = "application/json", StatusCode = 200 };
        }



        #endregion
    }
}