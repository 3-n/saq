using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SaqRepresentation
{
    public class ChoiceData
    {
        public Choice Choice { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public string Out { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    public class SelectableSolution : Solution
    {
        public List<Choice> Correct = new List<Choice>();

        public Dictionary<Choice, ChoiceData> ChoiceData
        {
            get
            {
                return Choices
                    .ToDictionary(pair => pair.Key, pair => new ChoiceData
                    {
                        Choice = pair.Key,
                        Text = pair.Value,
                        IsCorrect = Correct.Contains(pair.Key)
                    });
            }
        }

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

        public override bool IsPossible
        {
            get
            {
                return !Correct.Contains(Choice.X);
            }
        }

		public List<string> ChoicesSegmentPattern
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
    
}
