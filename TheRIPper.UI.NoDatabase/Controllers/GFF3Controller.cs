using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TheRIPper.BL.RIP;
using TheRIPper.BL.SequenceHelpers;
using TheRIPper.UI.NoDatabase.Models;

namespace TheRIPper.UI.NoDatabase.Controllers
{
    //[Route("api/[controller]/[action]")]
    [ApiController]
    public class GFF3Controller : ControllerBase
    {
        private IMemoryCache _cache;

        public GFF3Controller(IMemoryCache cache) {
            _cache = cache;
        }

        [HttpGet]
        [Route("api/gff3/file/{FileName}/{window}/{slide}/{compositeRequirement}/{productRequirement}/{substrateRequirement}/{compositeCountRequirement}/{checkGcContent}")]
        public IActionResult GFF3File(string FileName, int window, int slide, double compositeRequirement, double productRequirement, double substrateRequirement, int compositeCountRequirement, bool checkGcContent)
        {
            List<ISequence> sequences = SessionManagement
                .SessionMethods
                .Get<FileModels>(HttpContext.Session, FileName, false, _cache)
                .Sequences
                .Select(s => SequenceHelpers.BuildSequenceFromString(s.SequenceName, s.SequenceContent))
                .ToList();

            var builder = new StringBuilder();

            for (int x = 0; x < sequences.Count; x++) {
                if (x == 0)
                {
                    builder.AppendLine(
                        GFF3Logic.GFF3Sequence(sequences[x], window, slide, compositeRequirement, productRequirement, substrateRequirement, 0, true)
                        );
                }
                else {
                    builder.AppendLine(
                        GFF3Logic.GFF3Sequence(sequences[x], window, slide, compositeRequirement, productRequirement, substrateRequirement, 0, false)
                        );
                }
            }

            return File(Encoding.ASCII.GetBytes(builder.ToString()), "text/x-gff3", string.Join(".", FileName.Split(".")[0], "gff3"),true);
        }


        [HttpGet]
        [Route("api/gff3/sequence/{FileName}/{Sequence}/{window}/{slide}/{compositeRequirement}/{productRequirement}/{substrateRequirement}/{compositeCountRequirement}/{checkGcContent}")]
        public IActionResult GFF3Sequence(string FileName, string Sequence, int window, int slide, double compositeRequirement, double productRequirement, double substrateRequirement, int compositeCountRequirement, bool checkGcContent)
        {
            List<ISequence> sequences = SessionManagement
                .SessionMethods
                .Get<FileModels>(HttpContext.Session, FileName, false, _cache)
                .Sequences
                .Select(s => SequenceHelpers.BuildSequenceFromString(s.SequenceName, s.SequenceContent))
                .Where(w=>w.ID == Sequence)
                .ToList();

            var builder = new StringBuilder();

            for (int x = 0; x < sequences.Count; x++)
            {
                if (x == 0)
                {
                    builder.AppendLine(
                        GFF3Logic.GFF3Sequence(sequences[x], window, slide, compositeRequirement, productRequirement, substrateRequirement, 0, true)
                        );
                }
                else
                {
                    builder.AppendLine(
                        GFF3Logic.GFF3Sequence(sequences[x], window, slide, compositeRequirement, productRequirement, substrateRequirement, 0, false)
                        );
                }
            }

            return File(Encoding.ASCII.GetBytes(builder.ToString()), "text/x-gff3", string.Join(".", FileName.Split(".")[0] + "_" + Sequence, "gff3"), true);
        }



    }
}