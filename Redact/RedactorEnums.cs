using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redactorize.Redact
{
    public class RedactorEnums
    {
        public enum RedactionType
        {
            TextOnly,       // will read and redact only the text content of PDF
            ImageOnly,      // will read and redact only the image text content of PDF
            TextAndImage    // will read and redact both text and image text content of PDF
        }

        /// <summary>
        /// Use to define the matching strategy when parsing the PDF content(s).
        /// </summary>
        public enum RedactionMatchingStrategy
        {
            FixedPhrase,
            RegularExpression
        }

        public enum PageTextSource
        {
            Text,
            Image
        }

    }
}
