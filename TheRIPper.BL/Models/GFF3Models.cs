using System;
using System.Collections.Generic;
using System.Text;

namespace TheRIPper.BL.Models
{
    public class GFF3Models
    {
        public string SequenceId { get; set; }
        public string Source { get; set; }
        public string Type { get; set; } // rip affacted window
        public int Start { get; set; }
        public int End { get; set; }
        public double Score { get; set; }
        public char Strand { get; set; }
        public char Phase { get; set; }
        public string Attributes { get; set; }
    }
}



//seqid - name of the chromosome or scaffold; chromosome names can be given with or without the 'chr' prefix.Important note: the seq ID must be one used within Ensembl, i.e.a standard chromosome name or an Ensembl identifier such as a scaffold ID, without any additional content such as species or assembly.See the example GFF output below.
//source - name of the program that generated this feature, or the data source (database or project name)
//type - type of feature.Must be a term or accession from the SOFA sequence ontology
//start - Start position of the feature, with sequence numbering starting at 1.
//end - End position of the feature, with sequence numbering starting at 1.
//score - A floating point value.
//strand - defined as + (forward) or - (reverse).
//phase - One of '0', '1' or '2'. '0' indicates that the first base of the feature is the first base of a codon, '1' that the second base is the first base of a codon, and so on..
//attributes - A semicolon-separated list of tag-value pairs, providing additional information about each feature. Some of these tags are predefined, e.g.ID, Name, Alias, Parent - see the GFF documentation for more details.
