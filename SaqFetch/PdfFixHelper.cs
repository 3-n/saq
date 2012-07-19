using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;

namespace SaqFetch
{
    public static class PdfFixHelper
    {
        static PdfFixHelper()
        {

            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("SaqFetch.FixDictionary.txt");
            var reader = new StreamReader(stream);
            string theText = reader.ReadToEnd();

            var parsedLines = theText.Split(new[]{'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split(' '))
                .Select(line => new KeyValuePair<string, string>(line.First(), line.Last()));

            foreach (var pair in parsedLines)
            {
                if (!Dictionary.ContainsKey(pair.Key))
                {
                    Dictionary.Add(pair.Key, pair.Value);
                }
            }
        }

        private static readonly Dictionary<string, string> Dictionary = new Dictionary<string, string>();

        private static readonly HashSet<string> UnknownEntitiesInternal = new HashSet<string>();

        public static List<string> UnknownEntities
        {
            get { return UnknownEntitiesInternal.ToList(); }
        }

        public static string GetFixedString(string s)
        {
            return s
                .FixSergeantEntities()
                .FixNumberedEntities();
        }

        private static string FixSergeantEntities(this string s)
        {
            return Regex.Replace(s, "<[0-9a-fA-F]+>", match =>
            {
                var m = match.Captures[0].Value.Replace("<", "").Replace(">", "").ToLower();
                var sb = new StringBuilder();
                if (m.Length % 4 != 0)
                {
                    throw new FormatException(m + "is not a proper strange entity set");
                }
                for (var j = 0; j < m.Length; j += 4)
                {
                    if (Dictionary.ContainsKey(m.Substring(j, 4)))
                    {
                        sb.Append(Dictionary[m.Substring(j, 4)]);
                    }
                    else
                    {
                        sb.Append(m.Substring(j, 4));
                        UnknownEntitiesInternal.Add(m.Substring(j, 4));
                    }
                }

                return String.Format("({0})", sb.ToString());
            });
        }

        private static string FixNumberedEntities(this string s)
        {
            return Regex.Replace(s, "\\\\[0-9][0-9][0-9]", match =>
            {
                var m = match.Captures[0].Value;
                if(Dictionary.ContainsKey(m))
                {
                    return Dictionary[m];
                }
                else
                {
                    UnknownEntitiesInternal.Add(m);
                    return m;
                }
            });
        }

        public static string GetFixedContents(this PdfReader reader, int pageNumber)
        {
            return GetFixedString(Encoding.UTF7.GetString(reader.GetPageContent(pageNumber)));
        }
    }
}