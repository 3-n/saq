using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SaqRepresentation
{

    public class Solution
    {
        public virtual string UnsolvedText { get; set; }
        public virtual string SolvedText { get; set; }
        public virtual bool IsPossible
        {
            get
            {
                return true;
            }
        }

        public override string ToString()
        {
            return UnsolvedText;
        }

        public bool Equals(Solution other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.UnsolvedText, UnsolvedText) && Equals(other.SolvedText, SolvedText);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Solution)) return false;
            return Equals((Solution) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((UnsolvedText != null ? UnsolvedText.GetHashCode() : 0)*397) ^ (SolvedText != null ? SolvedText.GetHashCode() : 0);
            }
        }
    }
    
}
