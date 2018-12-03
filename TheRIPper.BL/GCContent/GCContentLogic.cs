using Bio;
using Bio.Extensions;
using System;
using System.Collections.Generic;

namespace TheRIPper.BL.GCContent
{
    public class GCContentLogic
    {
        /// <summary>
        /// Uses character search through a single sequence to calculate the GCContent
        /// </summary>
        /// <param name="sequence">ISequence File</param>
        /// <returns>GCContent (double)</returns>
        public static double GCContentSingleSequenceTotal(ISequence sequence) {
            char[] dnaSeq = sequence.ConvertToString().ToCharArray();
            decimal G_Frequency = 0;
            decimal C_Frequency = 0;
            for (int x = 0; x < dnaSeq.Length; x++) {
                if (x < dnaSeq.Length) {
                    if (dnaSeq[x] == 'G') {
                        G_Frequency++;
                    }
                }
                if (x < dnaSeq.Length) {
                    if (dnaSeq[x] == 'C') {
                        C_Frequency++;
                    }
                }
            }
            decimal GC_Content = ((G_Frequency + C_Frequency) / dnaSeq.Length) * 100;

            return (double)GC_Content;
        }

        /// <summary>
        /// Calculates the GCContent over a list of sequences using character search
        /// </summary>
        /// <param name="sequences">List of ISequence Files</param>
        /// <returns>GCContent</returns>
        public static double GCContentMultipleSequenceTotal(List<ISequence> sequences) {
            decimal G_Frequency = 0;
            decimal C_Frequency = 0;
            int totalLength = 0;

            foreach (var sequence in sequences) {
                char[] dnaSeq = sequence.ConvertToString().ToCharArray();

                totalLength += dnaSeq.Length;

                for (int x = 0; x < dnaSeq.Length; x++) {
                    if (x < dnaSeq.Length) {
                        if (dnaSeq[x] == 'G') {
                            G_Frequency++;
                        }
                    }
                    if (x < dnaSeq.Length) {
                        if (dnaSeq[x] == 'C') {
                            C_Frequency++;
                        }
                    }
                }
            }
            decimal GC_Content = ((G_Frequency + C_Frequency) / totalLength) * 100;
            return Math.Round((double)GC_Content, 2);
        }
    }
}