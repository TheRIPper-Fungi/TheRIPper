using TheRIPPer.Db.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheRIPper.Db.Data.Tables
{
    public class File
    {
        [Key]
        public int Id { get; set; }

        public string FileName { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }

        public string FkUserId { get; set; }

        public ICollection<Sequence> Sequences { get; set; }

        [ForeignKey("FkUserId")]
        public virtual ApplicationUser User { get; set; }
    }
}