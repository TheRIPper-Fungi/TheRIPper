using Bio;
using System;
using System.Collections.Generic;
using System.Text;
using TheRIPper.BL.Models;

namespace TheRIPper.BL.RIP
{
    public static class GFF3Logic
    {

        public static string GFF3Sequence(ISequence sequence, int window, int slidingSize, double compositeRequirement, double productRequirement, double substrateRequirement, int compositeCountRequirement, bool includeHeader) {
            List<Models.LRARModels> LRARs = LRARLogic.LRARSequence(sequence, window, slidingSize, compositeRequirement, productRequirement, substrateRequirement,compositeCountRequirement, false);
            List<GFF3Models> GFF3 = new List<GFF3Models>();
            //string.Join(" ", "Note=Product -", l.Product, "Substrate - ", l.Substrate, "Composite -", l.Composite, "Count -", l.Count)
            LRARs.ForEach(l => {
                var temp = new GFF3Models
                {
                    SequenceId = l.Name.Split(" ")[0],
                    Source = "theripper.hawk.rocks",
                    Type = "RIP",
                    Start = l.Start,
                    End = l.End,
                    Score = l.Composite,
                    Strand = '+',
                    Phase = '.',
                    Attributes = string.Join(" ", "Note=Product -", l.Product, "Substrate -", l.Substrate, "Composite -", l.Composite, "Count -", l.Count),
                };
                GFF3.Add(temp);
            });

            var builder = new StringBuilder();
            if (includeHeader) { builder.AppendLine("##gff3"); }

            for (int x = 0; x < GFF3.Count; x++) {
                var line = GFF3[x];
                builder.AppendLine(string.Join("\t", line.SequenceId, line.Source, line.Type, line.Start, line.End, line.Score, line.Strand, line.Phase, line.Attributes));
            }

            return builder.ToString();
        }

    }
}
