using System;
using Nancy;
using Nancy.Hosting.Self;
using SimpleWebQuiz;


namespace WebAppSelf
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            SimpleWebQuiz.SimpleWebQuiz stuff;
            StaticConfiguration.DisableCaches = true;
            var host = new NancyHost(new Uri("http://localhost:80"));
            host.Start();
            Console.ReadLine ();
            host.Stop();
        }
    }
}
