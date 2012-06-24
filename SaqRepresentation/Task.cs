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

		private Dictionary<Choice, string> unfixedChoices;
		public Dictionary<Choice, string> Choices 
		{
			get
			{
				return FixedChoices;
			}
			set
			{
				unfixedChoices = value;
			}
		}

		public override string UnsolvedText
        {
            get
            {
                var ret = FixedChoices
                    .Select(ch => String.Format("({0}) {1}", ch.Key, ch.Value))
                    .Aggregate((a, b) => a + "\n" + b);
                return ret;
            }
        }

        public override string SolvedText
        {
            get
            {
                return FixedChoices
                    .Where(ch => Correct.Contains(ch.Key))
                    .Select(ch => String.Format("({0}) {1}", ch.Key, ch.Value))
                    .Aggregate((a, b) => a + "\n" + b);
            }
        }

		private List<string> ChoicesSegmentPattern
		{
			get
			{
				var segmentedUnsolved = Regex
					.Replace(PreprocessedForSegmentation, @"\(([A-E])\).+(?=\t)|\(([A-E])\).+|(\t).+", @"$1$2$3")
					.Replace("\n", "")
					.ToCharArray();

				var ret = segmentedUnsolved
					.SelectMany(c => 
							!c.ToString().StartsWith("\t")
					            ? new []{c.ToString()}
					            : new []{"\t", "X"});

				var tmp = ret.ToArray();

				return ret.ToList();
			}
		}

		private List<string> ChoicesSegments
		{
			get
			{
				var segmentedUnsolved = Regex
                    .Replace(PreprocessedForSegmentation, @"\([A-E]\)\ (.+)(?=\t)|\([A-E]\)\ (.+)|(\t.+)", @"$1$2$3")
					.Split('\n');

				var ret = segmentedUnsolved
					.SelectMany(c => 
							!c.Contains("\t")
					            ? new []{c}
					            : new []{
									c.Split('\t').First(), 
									"\t", 
									c.Split('\t').Last()});

				var tmp = ret.ToArray();

				return ret.ToList();
			}
		}

		private Dictionary<Choice, string> FixedChoices 
		{
			get
			{
				var returnedChoices = new Dictionary<Choice, string>();
				if(ChoicesSegmentPattern.Contains("X"))
				{
					switch(ChoicesSegmentPattern.Aggregate((a, b) => a + b))
					{
					case "ADBEC\tX": //ADBEC E2
						returnedChoices = new Dictionary<Choice, string>()
						{
							{Choice.A, ChoicesSegments[0]},
							{Choice.D, ChoicesSegments[1]},
							{Choice.B, ChoicesSegments[2]},
							{Choice.E, ChoicesSegments[3]+" "+ChoicesSegments[6]},
							{Choice.C, ChoicesSegments[4]}
						};
						break;
					case "ADB\tXCE": //ADB D2CE
						returnedChoices = new Dictionary<Choice, string>()
						{
							{Choice.A, ChoicesSegments[0]},
							{Choice.D, ChoicesSegments[1]+" "+ChoicesSegments[4]},
							{Choice.B, ChoicesSegments[2]},
							{Choice.C, ChoicesSegments[5]},
							{Choice.E, ChoicesSegments[6]}
						};
						break;
					default:
						throw new NotSupportedException(String.Format(
							"Fix of SelectableSolution with pattern {0} is not yet available", 
							ChoicesSegmentPattern.Aggregate((a, b) => a + b)));
					}
				}
				else
				{
					returnedChoices = unfixedChoices;
				}

				return returnedChoices
					.OrderBy(pair => pair.Key)
					.ToDictionary(pair => pair.Key, pair => pair.Value);
			}
		}

		//change big gap before leftover segment into something sufficiently unique
		private string PreprocessedForSegmentation
		{
			get
			{

				var splat = UnfixedUnsolvedText
					.Split(new[]{"\r", "\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);
				var selected = splat
					.Select(line => line.Trim());
				var spaceNormalized = selected
                    .Aggregate((a, b) => a + "\n" + b);
				return Regex
					.Replace(spaceNormalized, @"\ \ \ \ +", "\t");
			}
		}

		private string UnfixedUnsolvedText
        {
            get
            {
                var ret = unfixedChoices
                    .Select(ch => String.Format("({0}) {1}", ch.Key, ch.Value))
                    .Aggregate((a, b) => a + "\n" + b);
                return ret;
            }
        }

        private string UnfixedSolvedText
        {
            get
            {
                return unfixedChoices
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
