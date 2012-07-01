using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SaqRepresentation
{

    public static class TranslationExtensions
    {
        public static string ToStringTranslated(this Category category)
        {
            return category.ToString();
        }
    }
}
