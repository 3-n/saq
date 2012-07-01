using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SaqFetch;
using SaqRepresentation;

namespace ConsoleViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            var testDoc = new Document("test.pdf");
            using (var outFile = File.CreateText("out.txt"))
            {
                foreach (var q in testDoc.Tasks)
                {
					if(new List<int>(){/*39, 46, 81, 97, 195*//*36,*/ 194, 195}.Contains(q.Number))
					{

	                        Console.WriteLine(q.Number);

							var selectable = q.Solution as SaqRepresentation.SelectableSolution;
							var choices = selectable.Choices;
							var tmp = choices.Select(ch => ch.Key.ToString()).ToList();

	                        Console.WriteLine(tmp.Aggregate((ch1, ch2) => ch1 + " " + ch2));
	                        Console.WriteLine(new[]{ 
	                    Choice.A, Choice.B, Choice.C, Choice.D, Choice.E}.Select(ch => ch.ToString()).Aggregate((ch1, ch2) => ch1 + " " + ch2));
	                    

						//var tmp = ((SaqRepresentation.SelectableSolution)q.Solution).ChoicesSegmentPattern.ToArray();
						//var tmp2 = ((SaqRepresentation.SelectableSolution)q.Solution).ChoicesSegments.ToArray();
						var a = 2;
                        outFile.Write(q.ToString());
					}
                    
                }
            }
			Console.Read();
        }
    }
}
