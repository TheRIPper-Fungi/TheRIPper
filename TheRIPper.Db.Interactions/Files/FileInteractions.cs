using TheRIPPer.Db.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using TheRIPper.Db.Data.Tables;

namespace TheRIPper.Db.Interactions.Files
{
    /// <summary>
    /// File Interactions Class Interface
    /// </summary>
    public interface IFile
    {
        IFile CreateFile();

        int? GetFileId();

        List<UserFile> List();

        bool RemoveFile();

        long GetFileBasePairs();
    }

    public class FileLogic : IFile
    {
        //Database class
        private ApplicationDbContext db = new ApplicationDbContext();

        //File properties
        public int? Id { get; private set; }
        public string FileName { get; private set; }
        public string Description { get; private set; }
        public string Location { get; private set; }
        public string UserId { get; private set; }

        //Empty Constructor
        public FileLogic() {
        }

        //Constructor for creating a new file
        public FileLogic(string FileName, string Description, string Location, string UserId) {
            this.FileName = FileName;
            this.Description = Description;
            this.Location = Location;
            this.UserId = UserId;
        }

        //Constructor used for getting a users files
        public FileLogic(string userId) {
            UserId = userId;
        }

        //Constructor used for getting a specific file
        public FileLogic(int Id) {
            this.Id = Id;
        }

        /// <summary>
        /// Save file on the system
        /// </summary>
        /// <returns>
        /// Returns FileLogic class
        /// </returns>
        public IFile CreateFile() {
            var file = new File {
                FileName = this.FileName,
                Description = this.Description,
                Location = this.Location,
                FkUserId = this.UserId
            };
            db.Files.Add(file);
            db.SaveChanges();

            this.Id = file.Id;

            return this;
        }

        /// <summary>
        /// List all of the Users Files, constructed with UserId
        /// </summary>
        /// <returns>List of Users files</returns>
        public List<UserFile> List() {
            var files = db.Files.Where(w => w.FkUserId == this.UserId)
                .Select(s => new UserFile {
                    Id = s.Id,
                    FileName = s.FileName,
                    Description = s.Description,
                    SequenceCount = s.Sequences.Count
                })
                .ToList();
            return files;
        }

        /// <summary>
        /// Used to get a files Id after it has been created
        /// </summary>
        /// <returns>FileId</returns>
        public int? GetFileId() {
            return this.Id;
        }

        //TODO: Remove the file from the filesystem to save storage space
        /// <summary>
        /// Remove a file from the database permenantly
        /// </summary>
        /// <returns>Bool confirmation of file deletion</returns>
        public bool RemoveFile() {
            try {
                int FileId = (int)this.Id;

                var Sequences = db.Sequences.Where(w => w.FkFileId == FileId).ToList();
                db.Sequences.RemoveRange(Sequences);

                var File = db.Files.Where(w => w.Id == FileId).FirstOrDefault();
                db.Files.Remove(File);

                db.SaveChanges();
                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        /// <summary>
        /// Get the amount of base pairs in the file
        /// </summary>
        /// <returns>Base Pairs count (long)</returns>
        public long GetFileBasePairs() {
            try {
                var fileBasePairs = db.Sequences.Where(w => w.FkFileId == this.Id).Sum(s => s.SequenceContent.Length);
                return fileBasePairs;
            }
            catch (Exception ex) {
                Console.WriteLine("ERROR : FileLogic.cs *** " + ex.ToString());

                return 0;
            }
        }


        public string GetFileName(int FileId) {
            try {
                return db.Files.Where(w => w.Id == FileId).Select(s => s.FileName).FirstOrDefault();
            }
            catch (Exception) {
                return "";
            }
        }
    }

    public class UserFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public int SequenceCount { get; set; }
    }
}
