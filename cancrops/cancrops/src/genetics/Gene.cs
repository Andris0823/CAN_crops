using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cancrops.src.genetics
{
    public class Gene
    {
        public Allele Dominant;
        public Allele Recessive;
        public string StatName;
        public bool Hidden;
        public Gene(string statName, Allele D, Allele R) 
        { 
            this.StatName = statName;
            this.Dominant = D;
            this.Recessive = R;
        }
        public Gene Clone()
        {
            return new Gene(this.StatName, new Allele(this.Dominant.Value), new Allele(this.Recessive.Value));
        }

    }
}
