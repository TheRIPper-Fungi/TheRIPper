using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TheRIPper.BL.SequenceHelpers;
using TheRIPper.Db.Interactions.Sequences;

namespace TheRIPper.AngularJs.Controllers
{
    public class SequenceController : Controller
    {
        public IActionResult Sequences() {
            return View();
        }

        [HttpGet]
        [Route("api/sequence/{FileId}")]
        public JsonResult GetSequencesByFileId(int FileId) {
            var sequences = SequenceInteractions.GetSequencesFromDatabaseByFileId(FileId);
            return new JsonResult(JsonConvert.SerializeObject(sequences)) { ContentType = "application/json", StatusCode = 200 };
        }
    }
}