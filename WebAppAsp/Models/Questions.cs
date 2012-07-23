using System;
using System.Linq;
using SaqRepresentation;
using System.Collections.Generic;
using SaqFetch;

namespace SimpleWebQuiz.Models
{
    public static class FakeDb
    {
        public static List<Task> Tasks;

        static FakeDb()
        {
            Tasks = new Document(new Uri("http://cem.edu.pl/lep_testy/12L1.pdf")).Tasks.ToList();
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

