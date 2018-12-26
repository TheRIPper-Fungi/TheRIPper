using Bio;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TheRIPper.BL.Models;
using TheRIPper.BL.RIP;
using TheRIPper.BL.SequenceHelpers;
using TheRIPper.Db.Interactions.Files;
using TheRIPper.Db.Interactions.Sequences;

namespace TheRIPper.AngularJs.Controllers
{
    public class RIPController : Controller
    {
        /// <summary>
        /// RIP Sequence view used for RIP sequence and RIP Genome Options
        /// </summary>
        /// <returns>View</returns>
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

        /// <summary>
        /// RIP Profile View
        /// </summary>
        /// <returns>View</returns>
        public IActionResult RIPProfileView() {
            return View();
        }

        //RIP Related API Methods

        #region API Endpoints

        //TODO: Confirm if needed and remove
        [HttpGet]
        [Route("api/rip")]
        public JsonResult Test() {
            List<RIPModels> rips = RIPLogic.RIPGenome(@"C:\Projects\Unrelated Projects\The RIPper\The RIPper\neurosporacrassa.fasta", 1000, 500);
            return new JsonResult(JsonConvert.SerializeObject(rips)) { ContentType = "application/json", StatusCode = 200 };
        }

        [HttpGet]
        [Route("api/rip/indexes/{FileId}")]
        public JsonResult GetRIPProfile(int FileId) {
            List<ISequence> sequences = SequenceInteractions.GetISequencesFromDatabaseByFileId(FileId);
            dynamic x = RIPLogic.SequenceRIPPercentages(sequences, 1000, 500, 0.3)
                .Select(s => new { SequenceName = s.SequenceName, RIPIndex = s.RIPIndex })
                .ToList();
            return new JsonResult(JsonConvert.SerializeObject(x)) { ContentType = "application/json", StatusCode = 200 };
        }

        [HttpGet]
        [Route("api/rip/file/{FileId}/{WindowSize?}/{SlidingSize?}")]
        public JsonResult RIPFile(int FileId, int? WindowSize, int? SlidingSize) {
            if (WindowSize == null) { WindowSize = 1000; };
            if (SlidingSize == null) { SlidingSize = 500; };

            List<ISequence> sequences = SequenceInteractions.GetISequencesFromDatabaseByFileId(FileId);
            List<RIPModels> RIPdata = RIPLogic.RIPGenome(sequences, (int)WindowSize, (int)SlidingSize);

            return new JsonResult(JsonConvert.SerializeObject(RIPdata)) { ContentType = "application/json", StatusCode = 200 };
        }

        [HttpGet]
        [Route("api/rip/sequence/{SequenceId}/{WindowSize?}/{SlidingSize?}")]
        public JsonResult Test(int SequenceId, int? WindowSize, int? SlidingSize) {
            if (WindowSize == null) { WindowSize = 1000; };
            if (SlidingSize == null) { SlidingSize = 500; };

            ISequence sequence = SequenceInteractions.GetSequenceBySequenceId(SequenceId);

            List<RIPModels> ripModels = RIPLogic.RIPSplitAndSequence(sequence, (int)WindowSize, (int)SlidingSize);

            return new JsonResult(JsonConvert.SerializeObject(ripModels)) { ContentType = "application/json", StatusCode = 200 };
        }

        [HttpGet]
        [Route("api/rip/lrar/sequence/{SequenceId}/{window}/{slide}/{compositeRequirement}/{compositeCountRequirement}")]
        public JsonResult LRARSequence(int SequenceId, int window, int slide, double compositeRequirement, int compositeCountRequirement) {

            ISequence sequence = SequenceInteractions.GetSequenceBySequenceId(SequenceId);

            List<LRARModels> LRARs = LRARLogic.LRARSequence(sequence, window, slide, compositeRequirement, compositeCountRequirement);

            return new JsonResult(JsonConvert.SerializeObject(LRARs)) { ContentType = "application/json", StatusCode = 200 };
        }

        [HttpGet]
        [Route("api/rip/lrar/file/{FileId}/{window}/{slide}/{compositeRequirement}/{compositeCountRequirement}")]
        public JsonResult LRARFile(int FileId, int window, int slide, double compositeRequirement, int compositeCountRequirement) {
            List<int> sequenceIds = SequenceInteractions.GetFileSequenceIds(FileId);

            List<LRARModels> LRARs = new List<LRARModels>();

            Enumerable.Range(0, sequenceIds.Count).AsParallel().ForAll(f => {
                var id = sequenceIds[f];
                ISequence sequence = SequenceInteractions.GetSequenceBySequenceId(id);

                LRARs.AddRange(LRARLogic.LRARSequence(sequence, window, slide, compositeRequirement, compositeCountRequirement));
            });


            //////////
            /////////
            ////////Single Threaded, not used anymore
            ///////
            //////
            //foreach (var id in sequenceIds) {
            //    LRARs.AddRange(LRARLogic.LRARSequence(id, window, slide, compositeRequirement, compositeCountRequirement));
            //}

            return new JsonResult(JsonConvert.SerializeObject(LRARs)) { ContentType = "application/json", StatusCode = 200 };
        }

        [HttpGet]
        [Route("api/rip/profile/file/{FileId}/{window}/{slide}/{compositeRequirement}/{compositeCountRequirement}")]
        public JsonResult RIPProfileFile(int FileId, int window, int slide, double compositeRequirement, int compositeCountRequirement) {

            List<ISequence> sequences = SequenceInteractions.GetISequencesFromDatabaseByFileId(FileId);
            string FileName = new FileLogic().GetFileName(FileId);
            
            var results = RIPProfileLogic.RIPFileProfile(sequences, window, slide, compositeRequirement, compositeCountRequirement,FileName);

            return new JsonResult(JsonConvert.SerializeObject(results)) { ContentType = "application/json", StatusCode = 200 };
        }

        #endregion API Endpoints
    }
}