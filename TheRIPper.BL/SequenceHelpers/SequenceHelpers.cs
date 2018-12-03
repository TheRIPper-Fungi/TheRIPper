using Bio;
using Bio.Algorithms.StringSearch;
using Bio.Extensions;
using Bio.IO;
using TheRIPPer.Db.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheRIPper.BL.GCContent;
using TheRIPper.BL.Models;

namespace TheRIPper.BL.SequenceHelpers
{
    public static class SequenceHelpers
    {
        /// <summary>
        /// Split a sequence into sub sequences based on the window and slide
        /// </summary>
        /// <param name="sequence">ISequence file</param>
        /// <param name="window">Window size</param>
        /// <param name="size">Sliding size</param>
        /// <returns></returns>
        public static List<SubSequenceModel> SplitSequence(ISequence sequence, int window, int size) {
            List<SubSequenceModel> retList = new List<SubSequenceModel>();
            for (int x = 0; x < sequence.Count; x += size) {
                //Make sure that there is enough sequence left to match window size
                if (sequence.Count - x > window) {
                    //Return the SubSequence
                    int start = x;
                    int end = x + window;
                    ISequence subSequence = sequence.GetSubSequence(start, window);
                    retList.Add(new SubSequenceModel {
                        SequenceName = sequence.ID,
                        Start = start,
                        End = end,
                        SubSequence = subSequence
                    });
                }
                else {
                    int remainingWindow = (int)(sequence.Count - x);
                    ISequence subSequence = sequence.GetSubSequence(x, remainingWindow);
                    retList.Add(new SubSequenceModel {
                        SequenceName = sequence.ID,
                        Start = x,
                        End = x + remainingWindow,
                        SubSequence = subSequence
                    });
                    //TODO: overflow logic
                }
            }
            return retList;
        }

        /// <summary>
        /// Uses boyer moore to find the positions of a searched sub sequence in a sequence
        /// </summary>
        /// <param name="sequence">ISequence</param>
        /// <param name="searchTerm">The sub sequence searched for</param>
        /// <returns>List of positions</returns>
        public static IEnumerable<int> SequenceSearchPositions(ISequence sequence, string searchTerm) {
            IPatternFinder searcher = new BoyerMoore() {
                IgnoreCase = true
            };
            return searcher.FindMatch(sequence, searchTerm);
        }

        /// <summary>
        /// Uses Boyer moore to file the position of searched sub sequences in a sequence
        /// </summary>
        /// <param name="sequence">ISequence</param>
        /// <param name="searchTerms">The sub sequences searched for</param>
        public static void SequenceBulkFrequencyMatch(ISequence sequence, List<string> searchTerms) {
            Parallel.ForEach(searchTerms, s => {
                var count = SequenceSearchPositions(sequence, s).ToList().Count();
                var c = count;
            });
        }

        /// <summary>
        /// Load FASTA file into a list of ISequence objects
        /// </summary>
        /// <param name="Filename">Name of the file</param>
        /// <returns>List of ISequence</returns>
        public static List<ISequence> LoadSequence(string Filename) {
            ISequenceParser parser = SequenceParsers.FindParserByFileName(Filename);
            List<ISequence> sequences = new List<ISequence>();
            if (parser == null) {
                //Something went wrong
            }
            else {
                using (parser.Open(Filename)) {
                    sequences = parser.Parse().ToList();
                }
                return sequences;
            }

            return null;
        }

        /// <summary>
        /// Merge list of ISequences into one ISequence
        /// </summary>
        /// <param name="sequences"></param>
        /// <returns></returns>
        public static ISequence MergeSequences(List<ISequence> sequences) {
            string sequenceStr = "";

            foreach (var s in sequences) {
                sequenceStr += s.ConvertToString();
            }

            ISequence mergedSequence = new Sequence(Alphabets.AmbiguousDNA, sequenceStr);

            return mergedSequence;
        }

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