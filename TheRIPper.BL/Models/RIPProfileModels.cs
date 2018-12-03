using System;
using System.Collections.Generic;
using System.Text;

namespace TheRIPper.BL.Models
{
    public class RIPProfileModels
    {
        public string FileName { get; set; }
        public long FileBP { get; set; }
        public int Count { get; set; }
        public double SumAverage { get; set; }
        public double ProductAverage { get; set; }
        public double SubstrateAverage { get; set; }
        public double CompositeAverage { get; set; }
        public int SumOfLRAR { get; set; }
        public double EstimatedGenomeRIP { get; set; }
        public int RIPPositiveWindows { get; set; }
        public double LRARAverageGCContent { get; set; }
        public double TotalGCContent { get; set; }
        public decimal WindowsInvestigated { get; set; }

    }
}
