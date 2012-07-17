using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SaqRepresentation;
using System.Text.RegularExpressions;

namespace SaqFetch
{
    public static class DocumentParsingExtensions
    {
        private static Random r = new Random();

        public static Choice ParseChoice(this string choice)
        {
            return (Choice)Enum.Parse(typeof(Choice), Regex.Match(choice, @"[a-zA-Z]").Value, true);
        }

        public static Task Task(this string task)
        {
            var rawProblem = task.Split(new[] { " A. " }, StringSplitOptions.RemoveEmptyEntries).First();
            var rawSolution = " A. " + task.Split(new[] { " A. " }, StringSplitOptions.RemoveEmptyEntries).Last();

            var rawSolutionTexts = rawSolution.Split(new[] { " A. ", " B. ", " C. ", " D. ", " E. " }, StringSplitOptions.RemoveEmptyEntries);

            var rawSolutionChoices = Regex.Matches(rawSolution, @"[A-E]{1}\.\ ");

            var tmp = (from Match rawSolutionChoice in rawSolutionChoices select rawSolutionChoice.Value.ParseChoice()).ToList().Distinct().ToList();

            var choiceDictionary = new Dictionary<Choice, string>();
            for (var i = 0; i < rawSolutionTexts.Length; i++)
            {
                choiceDictionary.Add(tmp[i], rawSolutionTexts[i]);
            }

            return new Task
            {
                Category = Category.NotSet,
                Number = Int32.Parse(new string(task.TakeWhile(c => c != '.').ToArray())),
                Problem = new Problem { Text = Regex.Replace(rawProblem, @"^[0-9]+\.\ +", "") },
                Solution = new SelectableSolution { Choices = choiceDictionary, Correct = new[] { (Choice)r.Next(5) }.ToList() }
            };
        }
    }
}
