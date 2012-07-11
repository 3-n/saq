using System;
using System.Linq;
using SaqRepresentation;
using System.Collections.Generic;

namespace SimpleWebQuiz.Models
{
    public class Questions
    {
        public List<Task> Tasks;
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
                return Tasks[perfectlyRandom];
            }
        }

        public string AnsweredCorrectly { get; set; }

        public Questions(string qid, string aid) : this(false)
        {
            perfectlyRandom = Int32.Parse(qid);
            var givenChoice = (Choice)Enum.Parse(typeof(Choice), aid);
            AnsweredCorrectly = (givenChoice == ((SelectableSolution)Tasks[perfectlyRandom].Solution).Correct.First()) ? "<h3><font color=\"green\">Correct!</font> :)</h3>" : "<h3><font color=\"red\">Incorrect!</font> :(</h3>";
            Console.WriteLine("q" + perfectlyRandom + " " + givenChoice + "vs" + ((SelectableSolution)Tasks[perfectlyRandom].Solution).Correct.First() + " --> " + AnsweredCorrectly);
            Console.WriteLine(String.Format("parsed {0} {1}", qid, perfectlyRandom));
        }

        public Questions(bool randomize=true)
        {
            Tasks = new List<Task>()
            {
                new Task()
                {
                    Problem = new Problem(){Text = "Która kończyna wychodzi u człowieka z dupy?"},
                    Solution = new SelectableSolution()
                    {
                        Choices = new Dictionary<Choice, string>()
                        {
                            {Choice.A, "Ręka"},
                            {Choice.B, "Noga"},
                            {Choice.C, "Żadna"},
                            {Choice.D, "Człowiek nie ma dupy"},
                            {Choice.E, "Odpowiedzi A i B są poprawne"},
                        },
                        Correct = new List<Choice>(){ Choice.B }
                    }
                },
                new Task()
                {
                    Problem = new Problem(){Text = "Ile pępków ma dziecko w wieku 7 lat?"},
                    Solution = new SelectableSolution()
                    {
                        Choices = new Dictionary<Choice, string>()
                        {
                            {Choice.A, "Nieskończenie wiele"},
                            {Choice.B, "Żadnego"},
                            {Choice.C, "1"},
                            {Choice.D, "2"},
                            {Choice.E, "3"},

                        },
                        Correct = new List<Choice>(){ Choice.C }
                    }
                },
                new Task()
                {
                    Problem = new Problem(){Text = "Która z części ciała kota nie występuje u każdego osobnika?"},
                    Solution = new SelectableSolution()
                    {
                        Choices = new Dictionary<Choice, string>()
                        {
                            {Choice.A, "Ogon"},
                            {Choice.B, "Wibrysy"},
                            {Choice.C, "Głowa"},
                            {Choice.D, "Łaty"},
                            {Choice.E, "Oko"},
                        },
                        Correct = new List<Choice>(){ Choice.D }
                    }
                },
                new Task()
                {
                    Problem = new Problem(){Text = "Czy kobiety pierdzą?"},
                    Solution = new SelectableSolution()
                    {
                        Choices = new Dictionary<Choice, string>()
                        {
                            {Choice.A, "Tak"},
                            {Choice.B, "Nie"},
                            {Choice.C, "Tylko pod prysznicem"},
                            {Choice.D, "Tylko lesbijki"},
                            {Choice.E, "Żadna odpowiedź nie jest prawidłowa"},
                        },
                        Correct = new List<Choice>(){ Choice.E }
                    }
                }
            };

            if(randomize)
            {
                var oldRandom = perfectlyRandom;
                while(oldRandom == perfectlyRandom)
                {
                    perfectlyRandom = r.Next(Tasks.Count);
                }
            }
        }
    }
}

