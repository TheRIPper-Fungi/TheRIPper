using System;
using System.Collections.Generic;
using System.Text;

namespace TheRIPper.BL.Models
{
    public class UserFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public int SequenceCount { get; set; }
    }
}
