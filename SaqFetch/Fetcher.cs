using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace SaqFetch
{
    public static class Fetcher
    {
        public static void DoStuff(string[] args)
        {
            var filePath = args.Any() ? args[0] : "test.pdf";
            var outFilePath = "out.pdf";

            var reader = new PdfReader(filePath);


            var pageCount = reader.NumberOfPages;

            //Console.WriteLine(PdfEncodings.IsPdfDocEncoding("UTF8"));
            //reader.SimpleViewerPreferences.
            for (int i = 6/*1*/; i <= 6/*pageCount*/; i++)
            {
                Console.WriteLine("<=== page {0}/{1} ===>", i, pageCount);

                var fixedContents = reader.GetFixedContents(i);
                var stringChars = new List<char>();


                var active = 0;
                var q = new Queue<char>(fixedContents);


                while (q.Count > 0)
                {
                    var current = q.Peek();

                    if (current == '(')
                    {
                        active++;
                        q.Dequeue();
                    }
                    if (current == ')')
                    {
                        active--;
                    }

                    if (active > 0)
                    {
                        if (q.Peek() == '\\')
                        {
                            q.Dequeue();
                        }
                        stringChars.Add(q.Dequeue());
                    }


                    if (active == 0 && q.Peek() == '<')
                    {
                        q.Dequeue();
                        var entity = new List<char>();
                        while (q.Peek() != '>')
                        {
                            entity.Add(q.Dequeue());
                        }
                        if (q.Peek() == 'T')
                        {
                            q.Dequeue();
                            if (q.Peek() == 'j')
                            {
                                stringChars.AddRange(entity);
                            }
                            q.Dequeue();
                        }
                    }

                    if (active == 0)
                    {
                        q.Dequeue();
                    }
                }




                Console.WriteLine(stringChars.Select(c => c.ToString()).Aggregate((a, b) => a + b));


                Console.WriteLine("<=== page {0}/{1} ===>", i, pageCount);


                using (var log = File.CreateText("alpha-out.txt"))
                {
                    log.Write(stringChars.Select(c => c.ToString()).Aggregate((a,b)=>a+b));
                }

                foreach (var unknownEntity in PdfFixHelper.UnknownEntities)
                {
                    Console.WriteLine(unknownEntity);
                }

                Console.ReadKey();
            }


            //Console.WriteLine(PdfEncodings.ConvertToString(reader.GetPageContent(i), BaseFont.CP1250));

        }


        //var a = Category.Cardiology;
        //Console.WriteLine(a.ToStringTranslated());
    }
}
