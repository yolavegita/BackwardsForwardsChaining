using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackwardsForwardsChaining.Models
{
    public class Rule
    {
        public int NumberNumeric { get; set; }
        public string Number { get; set; }
        public char RightSide { get; set; }
        public IEnumerable<char> LeftSide { get; set; }

        //For FC
        public bool Flag1 { get; set; }
        public bool Flag2 { get; set; }

        public override string ToString()
        {
            return string.Join(",", LeftSide) + " -> " + RightSide;
        }
    }
}
