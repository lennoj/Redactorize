using System.Text.RegularExpressions;
using iText.Kernel.Pdf;

namespace Redactorize.Redact
{
    public abstract class BasePageProcessor : IDisposable
    {
        public PdfPage? PdfPage { get; set; }
        public FileProcessor? FileProcessor { get; set; }

        public int PdfPageIndex { get; }

        protected BasePageProcessor(FileProcessor fileProcessor, PdfPage pdfPage, int pageIndex)
        {
            this.FileProcessor = fileProcessor;
            this.PdfPage = pdfPage; 
            this.PdfPageIndex = pageIndex;  
        }

        public abstract Task<List<PageTextPosition>> FindText(string textToFind, 
            RedactorEnums.RedactionMatchingStrategy findType = RedactorEnums.RedactionMatchingStrategy.FixedPhrase);

        public abstract Task FindTextAsync(string textToFind,
         RedactorEnums.RedactionMatchingStrategy findType = RedactorEnums.RedactionMatchingStrategy.FixedPhrase, Action<List<PageTextPosition>>? onCompleted = null);

        public static List<string> FindTextUsingRegEx(string textString, string regExPattern)
        {
            List<string> foundText = new List<string>();
            Regex regExInstance = new Regex(regExPattern, RegexOptions.IgnoreCase);
            MatchCollection matches = regExInstance.Matches(textString);

            if (matches.Count == 0)
                return foundText;

            foreach (Match match in matches)
            {
                foundText.Add(match.Value);
            }

            return foundText;
        }

        public static List<string> FindTextUsingFixedPhrase(string textString, string fixedPhrase)
        {
            List<string> foundText = new List<string>();
            string regExPattern = $@"\b{Regex.Escape(fixedPhrase)}\b";
            Regex regExInstance = new Regex(regExPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            MatchCollection matches = regExInstance.Matches(textString);

            if (matches.Count == 0)
                return foundText;

            foreach (Match match in matches)
            {
                foundText.Add(match.Value);
            }

            return foundText;
        }

        public void Dispose()
        {
            this.FileProcessor = null;
            this.PdfPage = null;
        }
    }
}
