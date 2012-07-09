using System;
using Nancy;
using Nancy.Hosting.Self;
using Nancy.Hosting.Aspnet;
using SimpleWebQuiz.Models;

namespace SimpleWebQuiz
{

	public class SimpleWebQuiz : NancyModule
	{
        Questions model;

		public SimpleWebQuiz()
		{
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
