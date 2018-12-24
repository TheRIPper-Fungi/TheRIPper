using Bio;
using Bio.Extensions;
using Bio.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheRIPper.BL.GCContent;
using TheRIPper.UI.NoDatabase.Models;

namespace TheRIPper.UI.NoDatabase.Controllers
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
                var files = collection.Files;

                var file = files[0];

                ISequenceParser parser = new Bio.IO.FastA.FastAParser();
                List<SequenceModels> sequences = new List<SequenceModels>();
                sequences = parser.Parse(file.OpenReadStream()).Select(s => new SequenceModels {
                    Id = s.ID,
                    SequenceContent = s.ConvertToString(),
                    SequenceName = s.ID,
                    GCContent = Math.Round(GCContentLogic.GCContentSingleSequenceTotal(s), 2)
                })
                .ToList();

                var fileObject = new FileModels {
                    FileName = file.FileName,
                    DateTimeAdded = DateTime.Now,
                    Sequences = sequences
                };

                if (SessionManagement
                    .SessionMethods
                    .Get<FileModels>(HttpContext.Session, file.FileName) == null) {
                    //if this file is not in session add it to the list of files
                    List<SessionFileModels> sessionFiles = SessionManagement
                        .SessionMethods
                        .Get<List<SessionFileModels>>(HttpContext.Session, "FileList");
                    if (sessionFiles != null) {
                        sessionFiles.Add(new SessionFileModels {
                            FileName = file.FileName,
                            SequenceCount = fileObject.Sequences.Count
                        });
                        SessionManagement.SessionMethods.Set<List<SessionFileModels>>(HttpContext.Session, "FileList", sessionFiles);
                    }
                    else {
                        List<SessionFileModels> newFileList = new List<SessionFileModels>();
                        newFileList.Add(new SessionFileModels {
                            FileName = file.FileName,
                            SequenceCount = fileObject.Sequences.Count
                        });
                        SessionManagement.SessionMethods.Set<List<SessionFileModels>>(HttpContext.Session, "FileList", newFileList);
                    }
                }

                SessionManagement.SessionMethods.Set<FileModels>(HttpContext.Session, file.FileName, fileObject);
            }
            catch (Exception ex) {
                return null;
            }

            return new JsonResult(JsonConvert.SerializeObject(true)) { ContentType = "application/json", StatusCode = 200 };

        }


        [HttpGet]
        [Route("api/files/list")]
        public JsonResult List() {

            var files = SessionManagement
                .SessionMethods
                .Get<List<SessionFileModels>>(HttpContext.Session, "FileList");

            return new JsonResult(JsonConvert.SerializeObject(new { files })) { ContentType = "application/json", StatusCode = 200 };

        }
    }
}