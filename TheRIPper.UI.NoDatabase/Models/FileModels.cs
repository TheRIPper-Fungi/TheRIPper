using Bio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheRIPper.UI.NoDatabase.Models
{
    public class FileModels
    {
        public string FileName { get; set; }

        public List<SequenceModels> Sequences { get; set; }

        public DateTime DateTimeAdded { get; set; }

    }

    public class UserFile
    {
        public string FileName { get; set; }
        public int SequenceCount { get; set; }
    }
}
