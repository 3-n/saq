using System;
using Nancy;
using Nancy.Hosting.Aspnet;
using SimpleWebQuiz.Models;
using System.Diagnostics;

namespace SimpleWebQuiz
{

	public class SimpleWebQuiz : NancyModule
	{
        Questions model;
        private static bool outputSupress = false;

		public SimpleWebQuiz()
		{
            if(!outputSupress)
            {
                InitWithLog();
            }

			Get["/"] = _ =>
			{
                Console.WriteLine("default");
                if(model==null)
                {
                    model = new Questions();
                }
				return View["Index", model];
			};

            Post["/{qid}/"] = parameters => 
            {
                Console.WriteLine("verification");
                var aid = Request.Form.aid.ToString();
                Console.WriteLine(aid);
                model = new Questions(parameters["qid"], aid);
                return View["Index", model];
            };
		}

        private static void InitWithLog()
        {
            var sw = new Stopwatch();
            sw.Start();
            var foo = FakeDb.Tasks.Count;
            Console.WriteLine("Initialized task DB in about {0}s", sw.Elapsed.TotalSeconds);
            outputSupress = true;
        }
	}
}

//
//    public class MainModule : NancyModule
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="NancyModule"/> class.
//        /// </summary>
//        public MainModule()
//        {
//            Get["/"] = (x) =>
//                {
//                    var model = new MainModel(
//                        "Jimbo", 
//                        new[] { new User("Bob", "Smith"), new User("Jimbo", "Jones"), new User("Bill", "Bobs"), },
//                        "<script type=\"text/javascript\">alert('Naughty JavaScript!');</script>");
//
//                    return View["Index", model];
//                };
//        }
//    }
