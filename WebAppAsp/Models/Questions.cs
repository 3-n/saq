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

            foreach (var task in Tasks)
            {
                ((SelectableSolution)task.Solution).Correct = new List<Choice> { Choice.D };
            }
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

        public string AnsweredCorrectly { get; set; }

        public Questions(string qid, string aid) : this(false)
        {
            perfectlyRandom = Int32.Parse(qid);
            var givenChoice = (Choice)Enum.Parse(typeof(Choice), aid);
            AnsweredCorrectly = (givenChoice == ((SelectableSolution)FakeDb.Tasks[perfectlyRandom].Solution).Correct.First()) ? "<h3><font color=\"green\">Correct!</font> :)</h3>" : "<h3><font color=\"red\">Incorrect!</font> :(</h3>";
            Console.WriteLine("q" + perfectlyRandom + " " + givenChoice + "vs" + ((SelectableSolution)FakeDb.Tasks[perfectlyRandom].Solution).Correct.First() + " --> " + AnsweredCorrectly);
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

