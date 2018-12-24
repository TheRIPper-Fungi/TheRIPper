using Bio;
using Bio.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheRIPper.BL.GCContent;
using TheRIPPer.Db.Data;

namespace TheRIPper.Db.Interactions.Sequences
{
    public class SequenceInteractions
    {
        #region Database Methods

        /// <summary>
        /// Creaate database entries for a list of sequences that are linked to an uploaded file in
        /// the database
        /// </summary>
        /// <param name="sequences">List of ISequence</param>
        /// <param name="FileId">The file Id</param>
        /// <returns>Bool to confirm creation</returns>
        public static bool AddSequencesToDatabase(List<ISequence> sequences, int FileId) {
            ApplicationDbContext db = new ApplicationDbContext();

            try {
                sequences.ForEach(s => {
                    var GCContent = GCContentLogic.GCContentSingleSequenceTotal(s);

                    var sequence = new Db.Data.Tables.Sequence {
                        FkFileId = FileId,
                        SequenceContent = s.ConvertToString(),
                        SequenceName = s.ID,
                        SequenceGCContent = GCContent
                    };
                    db.Sequences.Add(sequence);
                    db.SaveChanges();
                });
                return true;
            }
            catch (System.Exception) {
                return false;
            }
        }

        //TODO: Make object instead of dynamic
        /// <summary>
        /// Load List of sequences from the database based on the provided file Id
        /// </summary>
        /// <param name="FileId">File Id</param>
        /// <returns>Dynamic List of sequences</returns>
        public static dynamic GetSequencesFromDatabaseByFileId(int FileId) {
            ApplicationDbContext db = new ApplicationDbContext();

            var sequences = db.Sequences.Where(w => w.FkFileId == FileId).Select(s => new {
                Id = s.Id,
                Name = s.SequenceName,
                Length = s.SequenceContent.Length,
                GCContent = s.SequenceGCContent.HasValue ? (double)Math.Round((decimal)s.SequenceGCContent, 2) : s.SequenceGCContent
            })
            .OrderByDescending(o => o.Length)
            .ToList();

            return sequences;
        }

        /// <summary>
        /// Load List of sequences from the database based on the provided file Id
        /// </summary>
        /// <param name="FileId">File Id</param>
        /// <returns>List of ISequence</returns>
        public static List<ISequence> GetISequencesFromDatabaseByFileId(int FileId) {
            ApplicationDbContext db = new ApplicationDbContext();

            var sequences = db.Sequences.Where(w => w.FkFileId == FileId).Select(s => new Sequence(Alphabets.AmbiguousDNA, s.SequenceContent) { ID = s.SequenceName })
            .ToList();

            List<ISequence> ISequenceList = new List<ISequence>();
            ISequenceList.AddRange(sequences);

            return ISequenceList;
        }

        /// <summary>
        /// Returns a list of sequence Ids that are linked to a specific file
        /// </summary>
        /// <param name="FileId">File Id</param>
        /// <returns>List of Sequence Ids</returns>
        public static List<int> GetFileSequenceIds(int FileId) {
            ApplicationDbContext db = new ApplicationDbContext();

            return db.Sequences.Where(w => w.FkFileId == FileId).Select(s => s.Id).ToList();
        }

        /// <summary>
        /// Get a ISequence by the Sequence Id in the database
        /// </summary>
        /// <param name="Id">Sequence Id</param>
        /// <returns>ISequence</returns>
        public static ISequence GetSequenceBySequenceId(int Id) {
            ApplicationDbContext db = new ApplicationDbContext();

            var sequenceModel = db.Sequences.Where(w => w.Id == Id).FirstOrDefault();
            ISequence sequence = new Sequence(Alphabets.AmbiguousDNA, sequenceModel.SequenceContent) { ID = sequenceModel.SequenceName };
            return sequence;
        }

       

        #endregion Database Methods
    }
}
