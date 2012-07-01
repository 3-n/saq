using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SaqRepresentation
{
    public class Task
    {
        public DateTime Published { get; set; }
        public int Number { get; set; }
        public Problem Problem { get; set; }
        public Solution Solution { get; set; }
        public Category Category { get; set; }

        public bool Equals(Task other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Problem, Problem) && Equals(other.Solution, Solution);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Task)) return false;
            return Equals((Task) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Problem.GetHashCode()*397) ^ Solution.GetHashCode();
            }
        }

        public override string ToString()
        {
            return Problem + "\n" + Solution + "\n\n";
        }
    }






}
