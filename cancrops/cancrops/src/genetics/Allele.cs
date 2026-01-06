using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cancrops.src.genetics
{
    /// <summary>
    /// Represents a single allele (variant of a gene) with an integer value from 0-10.
    /// Higher values generally indicate better performance for that trait.
    /// </summary>
    public class Allele
    {
        /// <summary>
        /// The numeric value of this allele (0-10 range)
        /// </summary>
        public int Value;
        
        public Allele(int Val)
        {
            Value = Val;
        }
    }
}
