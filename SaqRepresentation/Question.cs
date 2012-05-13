using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaqRepresentation
{
    public class Question
    {
        public DateTime ExamDate { get; set; }
        public int Number { get; set; }
        public Category Category { get; set; }
        public string QuestionText { get; set; }
        public Answer Answer { get; set; }



        public bool Equals(Question other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.ExamDate.Equals(ExamDate) && other.Number == Number;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Question)) return false;
            return Equals((Question)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ExamDate.GetHashCode() * 397) ^ Number;
            }
        }
    }


    public enum Answer
    {
        A, B, C, D, E
    }

    public enum Category
    {
        Cardiology, Pediatry, Surgery
    }

    public static class TranslationExtensions
    {
        public static string ToStringTranslated(this Category category)
        {
            return category.ToString();
        }
    }
}
