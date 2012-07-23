using System;
using System.Linq;
using SaqRepresentation;
using System.Collections.Generic;
using SaqFetch;
using System.Net;

namespace SimpleWebQuiz.Models
{
    public static class FakeDb
    {
        public static List<Task> Tasks = new List<Task>();

        static FakeDb()
        {
            foreach(var year in Enumerable.Range(8, 5))
            {
                foreach(var no in new[]{1,2})
                {
                    try
                    {
                        Tasks.AddRange(new Document(new Uri(String.Format("http://cem.edu.pl/lep_testy/{0:00}L{1:0}.pdf", year, no))).Tasks);
                        Console.WriteLine("Processed {0:00}L{1:0}.pdf successfully", year, no);
                    }
                    catch(WebException e)
                    {
                        Console.WriteLine("{0:00}L{1:0}.pdf does not exist", year, no);
                    }
                }
            }
        }
    }

    public class DisplayChoiceData : ChoiceData
    {
        public Choice? UsersChoice { get; set; }
        public string Color
        {
            get
            {
                var highlight = IsCorrect ? "green;" : "red;";
                var ret = UsersChoice==Choice ? highlight : "lightgray;";
                return ret;
            }
        }

        public DisplayChoiceData(ChoiceData choiceData, Choice? usersChoice)
        {
            Choice = choiceData.Choice;
            Text = choiceData.Text;
            IsCorrect = choiceData.IsCorrect;
            UsersChoice = usersChoice;
        }
    }

    public class Questions
    {
        private Random r = new Random();
        private int perfectlyRandom;

        public int PerfectlyRandom
        {
            get
            {
                return perfectlyRandom;
            }
        }
        public Task RandomTask
        {
            get
            {
                return FakeDb.Tasks[perfectlyRandom];
            }
        }

        public string AnswerColor { get; set; }
        public bool Answered { get; set; }
        public Choice? GivenChoice { get; set; }

        public Dictionary<Choice, DisplayChoiceData> ChoicesDisplay
        {
            get
            {
                return ((SelectableSolution)RandomTask.Solution).ChoiceData
                    .ToDictionary(pair => pair.Key, pair => new DisplayChoiceData(pair.Value, GivenChoice));
            }
        }

        public int TaskCount
        {
            get
            {
                return FakeDb.Tasks.Count;
            }
        }

        public IEnumerable<Task> AllTasks
        {
            get
            {
                return FakeDb.Tasks;
            }
        }

        public Questions(string qid, string aid) : this(false)
        {
            perfectlyRandom = Int32.Parse(qid);
            GivenChoice = (Choice)Enum.Parse(typeof(Choice), aid);
            AnswerColor = (GivenChoice == ((SelectableSolution)FakeDb.Tasks[perfectlyRandom].Solution).Correct.First()) ? "green" : "red";
            Answered = true;
            Console.WriteLine("q" + perfectlyRandom + " " + GivenChoice + "vs" + ((SelectableSolution)FakeDb.Tasks[perfectlyRandom].Solution).Correct.First() + " --> " + AnswerColor);
            Console.WriteLine(String.Format("parsed {0} {1}", qid, perfectlyRandom));
        }

        public Questions(bool randomize=true)
        {
            if(randomize)
            {
                var oldRandom = perfectlyRandom;
                while(oldRandom == perfectlyRandom)
                {
                    perfectlyRandom = r.Next(FakeDb.Tasks.Count);
                }
            }
        }
    }
}

