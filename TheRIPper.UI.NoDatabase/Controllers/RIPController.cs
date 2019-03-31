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
        public JsonResult RIPSequence(string FileName, string SequenceName, int? WindowSize, int? SlidingSize) {
            if (WindowSize == null) { WindowSize = 1000; };
            if (SlidingSize == null) { SlidingSize = 500; };

            //In few cases the system fails to load the sequence,
            //Below code sets it to null and if not found it will stay null
            //If it stays null the code stops running and returns null
            //If not it carries on as normal
            SequenceModels sequenceObject = null;
            try {
                sequenceObject = SessionManagement
                        .SessionMethods.Get<FileModels>(HttpContext.Session, FileName)
                        .Sequences.Where(w => w.SequenceName == SequenceName)
                        .FirstOrDefault();
            }
            catch (Exception) {
            }
            if (sequenceObject == null) {
                return null;
            }
            else {
                ISequence sequence = SequenceHelpers.BuildSequenceFromString(sequenceObject.SequenceName, sequenceObject.SequenceContent);
                List<RIPModels> ripModels = RIPLogic.RIPSplitAndSequence(sequence, (int)WindowSize, (int)SlidingSize);
                return new JsonResult(JsonConvert.SerializeObject(ripModels)) { ContentType = "application/json", StatusCode = 200 };
            }
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
        [Route("api/rip/lrar/sequence/{FileName}/{SequenceName}/{window}/{slide}/{compositeRequirement}/{productRequirement}/{substrateRequirement}/{compositeCountRequirement}/{checkGCContent}")]
        public JsonResult LRARSequence(string FileName,string SequenceName, int window, int slide, double compositeRequirement,double productRequirement,double substrateRequirement, int compositeCountRequirement,bool checkGCContent) {

            ISequence sequence = SessionManagement
                .SessionMethods
                .Get<FileModels>(HttpContext.Session, FileName)
                .Sequences
                .Where(w => w.SequenceName == SequenceName)
                .Select(s => SequenceHelpers.BuildSequenceFromString(s.SequenceName, s.SequenceContent))
                .FirstOrDefault();

            List<LRARModels> LRARs = LRARLogic.LRARSequence(sequence, window, slide, compositeRequirement,productRequirement,substrateRequirement, compositeCountRequirement, checkGCContent);

            return new JsonResult(JsonConvert.SerializeObject(LRARs)) { ContentType = "application/json", StatusCode = 200 };
        }

        [HttpGet]
        [Route("api/rip/lrar/file/{FileName}/{window}/{slide}/{compositeRequirement}/{productRequirement}/{substrateRequirement}/{compositeCountRequirement}/{checkGCContent}")]
        public JsonResult LRARFile(string FileName, int window, int slide, double compositeRequirement,double productRequirement,double substrateRequirement, int compositeCountRequirement, bool checkGCContent) {
            List<ISequence> sequences = SessionManagement
                .SessionMethods
                .Get<FileModels>(HttpContext.Session, FileName)
                .Sequences
                .Select(s => SequenceHelpers.BuildSequenceFromString(s.SequenceName, s.SequenceContent))
                .ToList();

            List<LRARModels> LRARs = new List<LRARModels>();

            Enumerable.Range(0, sequences.Count).AsParallel().ForAll(f => {
                ISequence sequence = sequences[f];
                LRARs.AddRange(LRARLogic.LRARSequence(sequence, window, slide, compositeRequirement,productRequirement,substrateRequirement, compositeCountRequirement, checkGCContent));
            });

            return new JsonResult(JsonConvert.SerializeObject(LRARs)) { ContentType = "application/json", StatusCode = 200 };
        }

        [HttpGet]
        [Route("api/rip/profile/file/{FileName}/{window}/{slide}/{compositeRequirement}/{productRequirement}/{substrateRequirement}/{compositeCountRequirement}/{checkGcContent}")]
        public JsonResult RIPProfileFile(string FileName, int window, int slide, double compositeRequirement,double productRequirement, double substrateRequirement, int compositeCountRequirement,bool checkGcContent) {

            List<ISequence> sequences = SessionManagement
                .SessionMethods
                .Get<FileModels>(HttpContext.Session, FileName)
                .Sequences
                .Select(s => SequenceHelpers.BuildSequenceFromString(s.SequenceName, s.SequenceContent))
                .ToList();

            var results = RIPProfileLogic.RIPFileProfile(sequences, window, slide, compositeRequirement, productRequirement, substrateRequirement, compositeCountRequirement, FileName, checkGcContent);

            return new JsonResult(JsonConvert.SerializeObject(results)) { ContentType = "application/json", StatusCode = 200 };
        }

        [HttpGet]
        [Route("api/rip/file/RIPTotalPercentageWithGcContentValidityTest/{FileName}/{CompositeRequirement}/{ProductRequirement}/{SubstrateRequirement}/{WindowSize?}/{SlidingSize?}")]
        public JsonResult RIPTotalPercentageWithGcContentValidityTest(string FileName,double CompositeRequirement, double ProductRequirement, double SubstrateRequirement, int? WindowSize, int? SlidingSize) {
            if (WindowSize == null) { WindowSize = 1000; };
            if (SlidingSize == null) { SlidingSize = 500; };

            List<ISequence> sequences = SessionManagement
               .SessionMethods
               .Get<FileModels>(HttpContext.Session, FileName)
               .Sequences
               .Select(s => SequenceHelpers.BuildSequenceFromString(s.SequenceName, s.SequenceContent))
               .ToList();

            var RIPregions = RIPLogic.RIPGenome(sequences, (int)WindowSize, (int)SlidingSize);

            int countPositiveComposite = 0;
            foreach (var seq in sequences) {
                string current_seq_name = seq.ID;
                double current_seq_gc_content = BL.GCContent.GCContentLogic.GCContentSingleSequenceTotal(seq);
                int current_valid_seq_count = RIPregions.Where(w => w.SequenceName == current_seq_name && w.Composite >= CompositeRequirement && w.Product >= ProductRequirement && w.Substrate <= SubstrateRequirement && w.GCContent < current_seq_gc_content).Count();
                countPositiveComposite += current_valid_seq_count;
            }

            double RIPPercentageGenome = ((double)countPositiveComposite / (double)RIPregions.Count) * (double)100;

            return new JsonResult(JsonConvert.SerializeObject(RIPPercentageGenome)) { ContentType = "application/json", StatusCode = 200 };

        }



        #endregion
    }
}