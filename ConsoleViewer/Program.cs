using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SaqFetch;

namespace ConsoleViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Fetcher.DoStuff(args);
            var testDoc = new Document("test.pdf");
            using (var outFile = File.CreateText("out.txt"))
            {
                foreach (var q in testDoc.RawQuestionsText)
                {
                    outFile.WriteLine(q);
                    outFile.WriteLine();
                }
            }
        }
    }
}
