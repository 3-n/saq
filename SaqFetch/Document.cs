using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SaqRepresentation;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Diagnostics;

namespace SaqFetch
{
    public class Document
    {
        private readonly PdfReader taskReader;
        private readonly List<string> pages = new List<string>();
        private readonly Dictionary<int, Choice> solutions = new Dictionary<int, Choice>();
        private DateTime documentDate;

        public Document(string tasksFilePath, string solutionsFilePath = null) : this(
            new PdfReader(tasksFilePath),
            new PdfReader(solutionsFilePath ?? (tasksFilePath.Replace(".pdf", "_klucz.pdf"))))
        {
        }

        public Document(Uri tasksFile, Uri solutionsFile = null) : this(
            new PdfReader(tasksFile),
            new PdfReader(solutionsFile ?? new Uri(tasksFile.ToString().Replace(".pdf", "_klucz.pdf"))))
        {
        }

        public Document(PdfReader tasksFileReader, PdfReader solutionsFileReader)
        {
            taskReader = tasksFileReader;

            var answerKeyRaw = PdfTextExtractor.GetTextFromPage(solutionsFileReader, 1);
            answerKeyRaw = Regex.Replace(answerKeyRaw, @"\s", " ");
            var answerKeyMatches = Regex.Matches(answerKeyRaw, @"[1-9][0-9]*\ +[A-EX]");

            solutions = answerKeyMatches
                .Cast<Match>()
                .Select(m => m.Value.Split(new[]{" "}, StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(a => Int32.Parse(a.First()), a => (Choice)Enum.Parse(typeof(Choice), a.Last()));

            ProducePagedFixedText();

            //documentDate = DateTime.Parse(Regex.Match(pages.Last(), @"[\p{L}][\p{L}][\p{L}]+[\p{Z}]+[0-9]{4}").Value);
        }

        public IEnumerable<Task> Tasks
        {
            get
            {
                return RawTasksText
                    .Select(t => t.Task(solutions[t.TaskNumber()]))
                    .Where(task => task.Solution.IsPossible);
            }
        } 

        public string RawText 
        {
            get 
            { 
                return pages.Aggregate((a, b) => a + b);
            }
        }

        public List<string> RawTasksText
        {
            get
            {
                var tasks = new List<string>();
                foreach (var pageChunks in pages.Select(page => page.Split(new[] {" Nr "}, StringSplitOptions.RemoveEmptyEntries)))
                {
                    tasks.AddRange(pageChunks.Where(IsTask).Select(RemoveBeaurocracy));
                }
                return tasks;
            }
        }

        private bool IsTask(string textChunk)
        {
            return new[]
                    {
                        Regex.IsMatch(textChunk, @"^[0-9]+\."), //startsWithNumber
                        (new[] {"A", "B", "C", "D", "E"}) //hasAnswerIds
                            .All(letter => Regex.IsMatch(textChunk, String.Format(@"{0}\.", letter))),
                        textChunk.Length > 20, //isReasonablyLong
                    }
                .Aggregate((a, b) => a && b);
        }

        private string RemoveBeaurocracy(string rawQuestionText)
        {
            return Regex.Replace(rawQuestionText, "Weź teraz .+$", "").Replace("Dziękujemy!", "");
        }

        private void ProducePagedFixedText()
        {
            if(pages.Any())
            {
                return;
            }

            for (var i = 1; i <= taskReader.NumberOfPages; i++)
            {
                var stringChars = new List<char>();

                var active = 0;
                var q = new Queue<char>(taskReader.GetFixedContents(i));

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

                if(Debugger.IsAttached)
                {
                    using (var log = File.CreateText("unknown-entities.txt"))
                    {
                        foreach (var unknownEntity in PdfFixHelper.UnknownEntities)
                        {
                            log.WriteLine(unknownEntity);
                        }
                    }
                }

                pages.Add(stringChars.Select(c => c.ToString(CultureInfo.InvariantCulture)).Aggregate((a, b) => a + b));
            }
        }
    }
}
