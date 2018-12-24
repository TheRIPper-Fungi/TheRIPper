using Bio;
using Bio.Algorithms.StringSearch;
using Bio.Extensions;
using Bio.IO;
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

        public static ISequence BuildSequenceFromString(string name, string content) {
            ISequence sequence = new Sequence(Alphabets.AmbiguousDNA, content) {
                ID = name
            };
            return sequence;

        }

        
    }
}