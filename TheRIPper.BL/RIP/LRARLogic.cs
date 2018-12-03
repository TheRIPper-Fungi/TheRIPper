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
        public static List<LRARModels> LRARSequence(int SequenceId, int window, int slidingSize, double compositeRequirement, int compositeCountRequirement) {
            //This gets the LRARs (Large RIP Affected Regions for an entire sequence)

            var RIPResults = RIPLogic.RIPSequence(SequenceId, window, slidingSize);

            List<LRARModels> LRARs = new List<LRARModels>();

            int counter = 0;
            int? start = null;
            int? end = null;

            foreach (var RIPArea in RIPResults) {
                if (RIPArea.Composite > compositeRequirement && start == null && RIPArea.Product >= 1.1 && RIPArea.Substrate <= 0.9) {
                    start = RIPArea.Start;
                    counter++;
                }
                else if (RIPArea.Composite > compositeRequirement && start != null && RIPArea.Product >= 1.1 && RIPArea.Substrate <= 0.9) {
                    end = RIPArea.End;
                    counter++;
                }
                else {
                    if (start != null && end != null && counter > compositeCountRequirement) {
                        //get the product substrate and composite for this region
                        ISequence sequence = SequenceHelpers.SequenceHelpers.GetSequenceBySequenceId(SequenceId);

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