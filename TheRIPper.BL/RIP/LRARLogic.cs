using Bio;
using System;
using System.Collections.Generic;
using TheRIPper.BL.GCContent;
using TheRIPper.BL.Models;

namespace TheRIPper.BL.RIP
{
    public static class LRARLogic
    {
        /// <summary>
        /// Large RIP Affected Region analysis on a single sequence from the database
        /// </summary>
        /// <param name="SequenceId">The sequence Id stored in the database</param>
        /// <param name="window">The window</param>
        /// <param name="slidingSize">The sliding size</param>
        /// <param name="compositeRequirement">Composite requirements</param>
        /// <param name="compositeCountRequirement">Consequtive composite requirements</param>
        /// <returns>List of LRARModels</returns>
        public static List<LRARModels> LRARSequence(ISequence sequence, int window, int slidingSize, double compositeRequirement,double productRequirement,double substrateRequirement, int compositeCountRequirement,bool checkGCContent) {
            //Load the GCContent for the sequencec
            double sequence_gc_content = GCContent.GCContentLogic.GCContentSingleSequenceTotal(sequence);
            
            //This gets the LRARs (Large RIP Affected Regions for an entire sequence)

            var RIPResults = RIPLogic.RIPSplitAndSequence(sequence, window, slidingSize);

            List<LRARModels> LRARs = new List<LRARModels>();

            int counter = 0;
            int? start = null;
            int? end = null;

            foreach (var RIPArea in RIPResults) {
                bool valid_lrar = false;

                if(checkGCContent) valid_lrar = RIPArea.Composite > compositeRequirement && RIPArea.Product >= productRequirement && RIPArea.Substrate <= substrateRequirement && RIPArea.GCContent < sequence_gc_content;
                else valid_lrar = RIPArea.Composite > compositeRequirement && RIPArea.Product >= productRequirement && RIPArea.Substrate <= substrateRequirement;

                if (start == null && valid_lrar) {
                    start = RIPArea.Start;
                    counter++;
                }
                else if (start != null &&valid_lrar) {
                    end = RIPArea.End;
                    counter++;
                }
                else {
                    if (start != null && end != null && counter > compositeCountRequirement) {
                        //get the product substrate and composite for this region
                        

                        int size = (int)end - (int)start;

                        var subSequence = sequence.GetSubSequence((long)start, size);

                        var subSequenceRIPResult = RIPLogic.RIPSequence(subSequence, (int)start, (int)end);
                        var GCContent = Math.Round(GCContentLogic.GCContentSingleSequenceTotal(subSequence), 2);

                        LRARs.Add(new LRARModels {
                            Name = subSequenceRIPResult.SequenceName,
                            Start = (int)start,
                            End = (int)end,
                            Size = size,
                            Product = subSequenceRIPResult.Product,
                            Substrate = subSequenceRIPResult.Substrate,
                            Composite = subSequenceRIPResult.Composite,
                            GCContent = GCContent,
                            Count = counter
                        });

                        counter = 0;
                        start = null;
                        end = null;
                    }
                    else {
                        counter = 0;
                        start = null;
                        end = null;
                    }
                }
            }

            return LRARs;
        }
    }
}