using Bio;

namespace TheRIPper.BL.Models
{
    internal class SequenceModels
    {
    }

    public class SubSequenceModel
    {
        public string SequenceName { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public ISequence SubSequence { get; set; }
    }
}