using TheRIPPer.Db.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using TheRIPper.BL.Models;

namespace TheRIPper.BL.RIP
{
    public class RIPProfileLogic
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        //Make class models for the return
        public static RIPProfileModels RIPFileProfile(int FileId, int window, int slide, double compositeRequirement, int compositeCountRequirement) {
            List<int> sequenceIds = SequenceHelpers.SequenceHelpers.GetFileSequenceIds(FileId);

            //The below code focuses on the LRAR
            List<LRARModels> LRARs = new List<LRARModels>();

            Enumerable.Range(0, sequenceIds.Count).AsParallel().ForAll(f => {
                var LRAR_Range = LRARLogic.LRARSequence(sequenceIds[f], window, slide, compositeRequirement, compositeCountRequirement);
                LRARs.AddRange(LRAR_Range);
            });

            //////////
            /////////
            ////////Single Threaded, not used anymore
            ///////
            //////
            //foreach (int id in sequenceIds) {
            //    var LRAR_Range = LRARLogic.LRARSequence(id, window, slide, compositeRequirement, compositeCountRequirement);
            //    LRARs.AddRange(LRAR_Range);
            //}

            int CountOfLRAR = LRARs.Count;

            double LRARProductAverage = 0;
            double LRARSubstrateAverage = 0;
            double LRARCompositeAverage = 0;
            double LRARAverageSize = 0;

            //Make sure there was atleast 1 LRAR
            if (CountOfLRAR > 0) {
                LRARProductAverage = LRARs.Average(a => a.Product);
                LRARSubstrateAverage = LRARs.Average(a => a.Substrate);
                LRARCompositeAverage = LRARs.Average(a => a.Composite);
                LRARAverageSize = LRARs.Average(a => a.Size);
            }

            //Get the File total base pairs
            IFile file = new FileLogic(FileId);
            long totalBP = file.GetFileBasePairs();

            var RIPregions = RIPLogic.RIPGenome(SequenceHelpers.SequenceHelpers.GetISequencesFromDatabaseByFileId(FileId), window, slide);

            int countPositiveComposite = RIPregions.Where(w => w.Composite >= compositeRequirement && w.Product >= 1.1 && w.Substrate <= 0.9).Count();

            double RIPPercentageGenome = ((double)countPositiveComposite / (double)RIPregions.Count) * (double)100;

            double LRARAverageGCContent = LRARs.Count > 0 ? Math.Round(LRARs.Average(a => a.GCContent), 2) : 0;

            var sequences = SequenceHelpers.SequenceHelpers.GetISequencesFromDatabaseByFileId(FileId);
            double totalGCContent = GCContent.GCContentLogic.GCContentMultipleSequenceTotal(sequences);

            //How many windows where looked at, this is determined by the slide size though;
            decimal WindowsInvestigated = Math.Round((decimal)(totalBP / slide), 0);

            return new  RIPProfileModels{
                FileName = db.Files.Where(w => w.Id == FileId).Select(s => s.FileName).FirstOrDefault(),
                FileBP = totalBP,
                Count = CountOfLRAR,
                SumAverage = Math.Round(LRARAverageSize, 2),
                ProductAverage = Math.Round(LRARProductAverage, 2),
                SubstrateAverage = Math.Round(LRARSubstrateAverage, 2),
                CompositeAverage = Math.Round(LRARCompositeAverage, 2),
                SumOfLRAR = LRARs.Sum(s => s.Size),
                EstimatedGenomeRIP = Math.Round(RIPPercentageGenome, 2),
                RIPPositiveWindows = countPositiveComposite,
                LRARAverageGCContent = LRARAverageGCContent,
                TotalGCContent = totalGCContent,
                WindowsInvestigated = WindowsInvestigated
            };
        }
    }
}