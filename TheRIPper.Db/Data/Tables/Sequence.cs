using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheRIPper.Db.Data.Tables
{
    public class Sequence
    {
        //Unique Id for sequence
        [Key]
        public int Id { get; set; }

        //The Name of the sequence, from the fasta file
        public string SequenceName { get; set; }

        //The DNA string of the sequence
        public string SequenceContent { get; set; }

        //GC content for the sequence
        public double? SequenceGCContent { get; set; }

        //RIP content for the sequence
        public double? SequenceRIPContent { get; set; }

        public int FkFileId { get; set; }

        //The File that the sequence belongs to
        [ForeignKey("FkFileId")]
        public virtual File File { get; set; }
    }
}