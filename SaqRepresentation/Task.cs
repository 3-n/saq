using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    public class Problem
    {
        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }

        public bool Equals(Problem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Text, Text);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Problem)) return false;
            return Equals((Problem) obj);
        }

        public override int GetHashCode()
        {
            return (Text != null ? Text.GetHashCode() : 0);
        }
    }

    public class SelectableSolution : Solution
    {
        public List<Choice> Correct = new List<Choice>();
        public Dictionary<Choice, string> Choices = new Dictionary<Choice, string>();

        public override string UnsolvedText
        {
            get
            {
                var ret = Choices
                    .Select(ch => String.Format("({0}) {1}", ch.Key, ch.Value))
                    .Aggregate((a, b) => a + "\n" + b);
                return ret;
            }
        }

        public override string SolvedText
        {
            get
            {
                return Choices
                    .Where(ch => Correct.Contains(ch.Key))
                    .Select(ch => String.Format("({0}) {1}", ch.Key, ch.Value))
                    .Aggregate((a, b) => a + "\n" + b);
            }
        }
    }

    public class Solution
    {
        public virtual string UnsolvedText { get; set; }
        public virtual string SolvedText { get; set; }

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

    public enum Choice
    {
        A = 0, B, C, D, E
    }

    public enum Category
    {
        NotSet, Cardiology, Pediatry, Surgery
    }

    public static class TranslationExtensions
    {
        public static string ToStringTranslated(this Category category)
        {
            return category.ToString();
        }
    }
}
