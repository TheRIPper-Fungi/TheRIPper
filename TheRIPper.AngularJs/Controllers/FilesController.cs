using Bio;
using Bio.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TheRIPper.AngularJs.Models;
using TheRIPper.BL;
using TheRIPper.BL.SequenceHelpers;
using TheRIPper.Db.Interactions.Files;
using TheRIPper.Db.Interactions.Sequences;

namespace TheRIPper.AngularJs.Controllers
{
    [Produces("application/json")]
    public class FilesController : Controller
    {
        private IHostingEnvironment _env;

        public FilesController(IHostingEnvironment env) {
            _env = env;
        }

        public IActionResult Files() {
            return View();
        }

        //https://stackoverflow.com/questions/38144194/iformfile-is-always-empty-in-asp-net-core-webapi
        [HttpPost]
        [DisableRequestSizeLimit]
        [Route("api/files/upload")]
        //public async Task<IActionResult> UploadFasta(IFormCollection collection) {
        public async Task<JsonResult> UploadFasta(IFormCollection collection) {
            try {
                var webRoot = _env.WebRootPath;
                string FastaName = Path.GetRandomFileName() + ".fasta";
                var filePath = System.IO.Path.Combine(webRoot + string.Format("{0}files{0}fasta{0}", Path.DirectorySeparatorChar), FastaName);

                var files = collection.Files;

                var file = files[0];


                if (file.Length > 0) {
                    using (var stream = new FileStream(filePath, FileMode.Create)) {
                        await file.CopyToAsync(stream);
                    }
                }

                return new JsonResult(JsonConvert.SerializeObject(new { FileName = FastaName })) { ContentType = "application/json", StatusCode = 200 };
            }
            catch (Exception ex) {
                return new JsonResult(JsonConvert.SerializeObject(new { ex = ex })) { ContentType = "application/json", StatusCode = 200 };
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/files/save")]
        public JsonResult SaveFile([FromBody] FileModels fileModel) {
            string UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (UserId != null) {
                FileLogic file = new FileLogic(fileModel.FileName,
                        fileModel.Description,
                        fileModel.Location,
                        UserId);

                int? CreatedFileId = file.CreateFile().GetFileId();

                bool IsCreated = CreatedFileId != null;
                bool SequencesStored = false;
                if (CreatedFileId != null) {
                    var webRoot = _env.WebRootPath;

                    var filePath = System.IO.Path.Combine(webRoot + string.Format("{0}files{0}fasta{0}", Path.DirectorySeparatorChar), fileModel.Location);
                    SequencesStored = SequenceInteractions.AddSequencesToDatabase(SequenceHelpers.LoadSequence(filePath), (int)CreatedFileId);
                }

                return new JsonResult(JsonConvert.SerializeObject(new { IsCreated, SequencesStored })) { ContentType = "application/json", StatusCode = 200 };
            }
            else {
                return new JsonResult(JsonConvert.SerializeObject(new { Error = "User Not Logged In" })) { ContentType = "application/json", StatusCode = 200 };
            }
        }

        [HttpGet]
        [Route("api/files/list")]
        public JsonResult List() {
            string UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId != null) {
                IFile file = new FileLogic(UserId);
                var files = file.List();
                return new JsonResult(JsonConvert.SerializeObject(new { files })) { ContentType = "application/json", StatusCode = 200 };
            }
            else {
                return new JsonResult(JsonConvert.SerializeObject(new { Error = "User Not Logged In" })) { ContentType = "application/json", StatusCode = 200 };
            }
        }

        [HttpDelete]
        [Route("api/files/remove/{FileId}")]
        public JsonResult Remove(int FileId) {
            IFile file = new FileLogic(FileId);
            bool IsRemoved = file.RemoveFile();
            return new JsonResult(JsonConvert.SerializeObject(new { IsRemoved })) { ContentType = "application/json", StatusCode = 200 };
        }

        
    }
}