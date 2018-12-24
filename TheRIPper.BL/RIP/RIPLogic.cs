using Bio;
using Bio.Algorithms.StringSearch;
using Bio.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TheRIPper.BL.GCContent;
using TheRIPper.BL.Models;

namespace TheRIPper.BL.RIP
{
    public static class RIPLogic
    {
        /// <summary>
        /// Not Used!
        /// This method uses the Boyer Moore Method of finding counts of Subsequences in the
        /// FASTA File sequence provided, it is slower than using a character search as
        /// it uses fuzzy search logic
        /// </summary>
        /// <param name="sequence">The DNA sequence which is searched</param>
        /// <param name="start">The start position</param>
        /// <param name="end">The end position</param>
        /// <returns>Returns a RIPmodels object</returns>
        public static RIPModels RIPSequenceBoyerMoore(ISequence sequence, int start, int end) {
            //The pattern case should be ignored
            IPatternFinder searcher = new BoyerMoore() {
                IgnoreCase = true
            };

            //Counts specfic DNA sub sequences in the sequence provided
            double TA_F = searcher.FindMatch(sequence, "TA").ToList().Count;
            double AT_F = searcher.FindMatch(sequence, "AT").ToList().Count;
            double CA_F = searcher.FindMatch(sequence, "CA").ToList().Count;
            double TG_F = searcher.FindMatch(sequence, "TG").ToList().Count;
            double AC_F = searcher.FindMatch(sequence, "AC").ToList().Count;
            double GT_F = searcher.FindMatch(sequence, "GT").ToList().Count;

            //RIP formula applied to the counts for the sub sequences count
            double productIndex = Math.Round(AT_F > 0 ? TA_F / AT_F : -1, 6);
            double substrateIndex = Math.Round((AC_F + GT_F) > 0 ? ((CA_F + TG_F) / (AC_F + GT_F)) : -1, 6);
            double compositeIndex = Math.Round(productIndex - substrateIndex, 6);

            //Return the calculated results
            return new RIPModels { SequenceName = sequence.ID, Start = start, End = end, Product = productIndex, Substrate = substrateIndex, Composite = compositeIndex };
        }

        /// <summary>
        /// Uses character search to find the required sub sequences and calculate the RIP
        /// for the provided sequence, faster than Boyer Moore as it does not use any fuzzy search logic
        /// and was crafted for this specific use case
        /// </summary>
        /// <param name="sequence">The DNA sequence which is searched</param>
        /// <param name="start">The start position</param>
        /// <param name="end">The end position</param>
        /// <returns>The end position</returns>
        public static RIPModels RIPSequence(ISequence sequence, int start, int end) {
            //Convert the sequence to an array of characters
            char[] dnaSeq = sequence.ConvertToString().ToCharArray();

            //Instantiate the counter variables
            double TA_f = 0;
            double AT_f = 0;
            double CA_F = 0;
            double TG_F = 0;
            double AC_F = 0;
            double GT_F = 0;

            //Loop through each character in the character array
            for (int x = 0; x < dnaSeq.Length; x++) {
                //Check if this is not the last character of the array, as if it
                //is the last, the next character cannot be checked as it doesn't exist
                if (x < dnaSeq.Length - 1) {
                    //Each if else if looks at the character in the current position and
                    //the characted in the next position, if it matches it increments the
                    //count for the specific sub sequence
                    if (dnaSeq[x] == 'T' && dnaSeq[x + 1] == 'A') {
                        TA_f++;
                    }
                    else if (dnaSeq[x] == 'A' && dnaSeq[x + 1] == 'T') {
                        AT_f++;
                    }
                    else if (dnaSeq[x] == 'C' && dnaSeq[x + 1] == 'A') {
                        CA_F++;
                    }
                    else if (dnaSeq[x] == 'T' && dnaSeq[x + 1] == 'G') {
                        TG_F++;
                    }
                    else if (dnaSeq[x] == 'A' && dnaSeq[x + 1] == 'C') {
                        AC_F++;
                    }
                    else if (dnaSeq[x] == 'G' && dnaSeq[x + 1] == 'T') {
                        GT_F++;
                    }
                }
                else {
                    //Last character, no increment required
                }
            }

            //RIP formula applied to the counts for the sub sequences count
            double productIndex = AT_f > 0 ? TA_f / AT_f : -1;
            double substrateIndex = (AC_F + GT_F) > 0 ? ((CA_F + TG_F) / (AC_F + GT_F)) : -1;
            double compositeIndex = productIndex - substrateIndex;

            //Return the calculated results, rounded to 2 decimal places
            return new RIPModels {
                SequenceName = sequence.ID,
                Start = start,
                End = end,
                Product = Math.Round(productIndex, 2),
                Substrate = Math.Round(substrateIndex, 2),
                Composite = Math.Round(compositeIndex, 2)
            };
        }

        /// <summary>
        /// Uses character search to find the required sub sequences and calculate the RIP
        /// for the provided sequence, faster than Boyer Moore as it does not use any fuzzy search logic
        /// and was crafted for this specific use case
        /// </summary>
        /// <param name="SequenceId">The Database Id of the sequence</param>
        /// <param name="window">The amount of Base Pairs to inspect for each sub sequence</param>
        /// <param name="slidingSize">The amount of Base Pairs to slide forwards for each new window, usally half of the window size</param>
        /// <returns>Returns a list of RIPModel objects</returns>
        public static List<RIPModels> RIPSplitAndSequence(ISequence sequence, int window, int slidingSize) {

            List<SubSequenceModel> subSequences = new List<SubSequenceModel>();
            List<RIPModels> ripModels = new List<RIPModels>();

            subSequences.AddRange(SequenceHelpers.SequenceHelpers.SplitSequence(sequence, window, slidingSize));

            subSequences.ForEach(ss => {
                var ripResult = RIPLogic.RIPSequence(ss.SubSequence, ss.Start, ss.End);
                var GCContent = Math.Round(GCContentLogic.GCContentSingleSequenceTotal(ss.SubSequence), 2);
                ripResult.GCContent = GCContent;
                ripModels.Add(ripResult);

                //csv += $"{ss.SequenceName}|{ss.Start}|{ss.End}|{rip.Product}|{rip.Substrate}|{rip.Composite}\n";
            });

            return ripModels;
        }

        /// <summary>
        /// Uses the file path provided to apply RIP to an entire FASTA file by running it through
        /// the RIPSequence method
        /// </summary>
        /// <param name="filePath">The file path for the genome in FASTA format</param>
        /// <param name="window">The amount of Base Pairs to inspect for each sub sequence</param>
        /// <param name="slidingSize">The amount of Base Pairs to slide forwards for each new window, usally half of the window size</param>
        /// <returns></returns>
        public static List<RIPModels> RIPGenome(string filePath, int window, int slidingSize) {
            //Load the file into an array of sequences, based on the FASTA file markings
            List<ISequence> sequences = SequenceHelpers.SequenceHelpers.LoadSequence(filePath);

            //Instantiate a List of sub sequence models
            List<SubSequenceModel> subSequences = new List<SubSequenceModel>();

            //Instantiate a list of RIP Models
            List<RIPModels> ripModels = new List<RIPModels>();

            //Move sequences into sub sequence models objects
            sequences.ForEach(sequence => {
                subSequences.AddRange(SequenceHelpers.SequenceHelpers.SplitSequence(sequence, window, slidingSize));
            });

            //for each sub sequence model, run RIPSequence and add it to the list of RIP Models
            subSequences.ForEach(ss => {
                var ripResult = RIPLogic.RIPSequence(ss.SubSequence, ss.Start, ss.End);

                ripModels.Add(ripResult);

                //csv += $"{ss.SequenceName}|{ss.Start}|{ss.End}|{rip.Product}|{rip.Substrate}|{rip.Composite}\n";
            });

            //return the results
            return ripModels;
        }

        /// <summary>
        /// Uses a list of sequences (entire genome / file) and runs the RIP calculations
        /// on it
        /// </summary>
        /// <param name="sequences">A list of genome Seqences</param>
        /// <param name="window">The amount of Base Pairs to inspect for each sub sequence</param>
        /// <param name="slidingSize">The amount of Base Pairs to slide forwards for each new window, usally half of the window size</param>
        /// <returns></returns>
        public static List<RIPModels> RIPGenome(List<ISequence> sequences, int window, int slidingSize) {
            //Instantiate a List of sub sequence models
            List<SubSequenceModel> subSequences = new List<SubSequenceModel>();

            List<RIPModels> rips = new List<RIPModels>();

            sequences.ForEach(sequence => {
                subSequences.AddRange(SequenceHelpers.SequenceHelpers.SplitSequence(sequence, window, slidingSize));
            });



            subSequences.ForEach(ss => {
                var ripResult = RIPLogic.RIPSequence(ss.SubSequence, ss.Start, ss.End);
                ripResult.GCContent = Math.Round(GCContentLogic.GCContentSingleSequenceTotal(ss.SubSequence), 2);
                rips.Add(ripResult);

                //csv += $"{ss.SequenceName}|{ss.Start}|{ss.End}|{rip.Product}|{rip.Substrate}|{rip.Composite}\n";
            });

            return rips;
        }

        public static List<(string SequenceName, double RIPIndex)> SequenceRIPPercentages(List<ISequence> sequences, int window, int slidingSize, double compositeLevel) {
            List<(string SequenceName, double RIPIndex)> ripIndexes = new List<(string SequenceName, double RIPIndex)>();

            List<RIPModels> ripResults = RIPGenome(sequences, window, slidingSize);
            List<string> sequenceNames = ripResults.Select(s => s.SequenceName).Distinct().ToList();

            foreach (var sequenceName in sequenceNames) {
                List<RIPModels> currentSequence = ripResults.Where(w => w.SequenceName == sequenceName).ToList();

                int countRIP = currentSequence.Count(w => w.Composite >= compositeLevel);
                int countNonRIP = currentSequence.Count(c => c.Composite < compositeLevel);

                double ripIndex = ((double)countRIP / (double)currentSequence.Count) * (double)100;

                ripIndexes.Add((sequenceName, ripIndex));
            }

            return ripIndexes;
        }
    }
}