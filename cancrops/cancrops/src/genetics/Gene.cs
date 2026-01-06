using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cancrops.src.genetics
{
    /// <summary>
    /// Represents a single genetic trait with dominant and recessive alleles.
    /// In Mendelian genetics, the dominant allele determines the expressed trait (phenotype).
    /// Each gene corresponds to a crop statistic (gain, growth, strength, resistance, fertility, mutativity).
    /// </summary>
    public class Gene
    {
        /// <summary>
        /// The dominant allele - this value is used for the expressed trait
        /// </summary>
        public Allele Dominant;
        
        /// <summary>
        /// The recessive allele - stored but only expressed if no dominant allele present
        /// </summary>
        public Allele Recessive;
        
        /// <summary>
        /// Name of the stat this gene controls (e.g., "gain", "growth", "strength")
        /// </summary>
        public string StatName;
        
        /// <summary>
        /// Whether this gene should be hidden in the UI
        /// </summary>
        public bool Hidden;
        
        public Gene(string statName, Allele D, Allele R) 
        { 
            this.StatName = statName;
            this.Dominant = D;
            this.Recessive = R;
        }
        
        /// <summary>
        /// Creates a deep copy of this gene
        /// </summary>
        public Gene Clone()
        {
            return new Gene(this.StatName, new Allele(this.Dominant.Value), new Allele(this.Recessive.Value));
        }

    }
}
