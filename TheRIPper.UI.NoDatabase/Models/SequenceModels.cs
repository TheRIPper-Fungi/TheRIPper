using Bio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheRIPper.UI.NoDatabase.Models
{
    public class SequenceModels
    {
        public string Id { get; set; }
        public string SequenceName { get; set; }
        public double GCContent { get; set; }
        public string SequenceContent { get; set; }
    }
}
