using Bio;
using Bio.Extensions;
using Bio.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
        private IMemoryCache _cache;

        public FilesController(IHostingEnvironment env, IMemoryCache cache) {
            _env = env;
            _cache = cache;
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

                for (int findex = 0; findex < files.Count; findex++) {
                    var file = files[findex];

                    ISequenceParser parser = new Bio.IO.FastA.FastAParser();
                    List<SequenceModels> sequences = new List<SequenceModels>();
                    sequences = parser.Parse(file.OpenReadStream()).Select(s => new SequenceModels
                    {
                        Id = s.ID,
                        SequenceContent = s.ConvertToString(),
                        SequenceName = s.ID,
                        GCContent = Math.Round(GCContentLogic.GCContentSingleSequenceTotal(s), 2)
                    })
                    .ToList();

                    var fileObject = new FileModels
                    {
                        FileName = file.FileName,
                        DateTimeAdded = DateTime.Now,
                        Sequences = sequences
                    };

                    if (SessionManagement
                        .SessionMethods
                        .Get<FileModels>(HttpContext.Session, file.FileName, false, _cache) == null)
                    {
                        //if this file is not in session add it to the list of files
                        List<SessionFileModels> sessionFiles = SessionManagement
                            .SessionMethods
                            .Get<List<SessionFileModels>>(HttpContext.Session, "FileList", true, null);
                        if (sessionFiles != null)
                        {
                            sessionFiles.Add(new SessionFileModels
                            {
                                FileName = file.FileName,
                                SequenceCount = fileObject.Sequences.Count
                            });
                            SessionManagement.SessionMethods.Set<List<SessionFileModels>>(HttpContext.Session, "FileList", sessionFiles, true, _cache);
                        }
                        else
                        {
                            List<SessionFileModels> newFileList = new List<SessionFileModels>();
                            newFileList.Add(new SessionFileModels
                            {
                                FileName = file.FileName,
                                SequenceCount = fileObject.Sequences.Count
                            });
                            SessionManagement.SessionMethods.Set<List<SessionFileModels>>(HttpContext.Session, "FileList", newFileList, true, _cache);
                        }
                    }

                    SessionManagement.SessionMethods.Set<FileModels>(HttpContext.Session, file.FileName, fileObject, false, _cache);
                }
                
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
                .Get<List<SessionFileModels>>(HttpContext.Session, "FileList", true, null);

            return new JsonResult(JsonConvert.SerializeObject(new { files })) { ContentType = "application/json", StatusCode = 200 };

        }
    }
}