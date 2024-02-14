using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using Redactorize.RedactText;

namespace Redactorize.Redact
{
    internal class PageTextProcessor :BasePageProcessor, IDisposable
    {
        public PageTextProcessor(FileProcessor fileProcessor, PdfPage pdfPage , int pageIndex) : base(fileProcessor, pdfPage, pageIndex)
        {

        }

        public override async Task<List<PageTextPosition>> FindText(string textToFind, 
            RedactorEnums.RedactionMatchingStrategy findType = RedactorEnums.RedactionMatchingStrategy.FixedPhrase)
        {
            List<PageTextPosition> textPositions = new List<PageTextPosition>();
            List<string> matches = new List<string> ();
            if (this.PdfPage == null)
                return textPositions;

            var strategy = new SimpleTextExtractionStrategy();
            var pdfCanvasProcessor = new PdfCanvasProcessor(strategy);

            pdfCanvasProcessor.ProcessPageContent(this.PdfPage);
            string result = strategy.GetResultantText();

            if (findType == RedactorEnums.RedactionMatchingStrategy.FixedPhrase) {
                matches = BasePageProcessor.FindTextUsingFixedPhrase(result, textToFind);
            }   
            else if(findType == RedactorEnums.RedactionMatchingStrategy.RegularExpression)
            {
                matches = BasePageProcessor.FindTextUsingRegEx(result, textToFind);
            }

            foreach(string match in matches.Distinct<string>())
            {
                List<PageTextPosition> textPosition =  TextPositionFinderStrategy.GetTextPosition(match, this.PdfPage, this.PdfPageIndex);
                textPositions.AddRange(textPosition);
            }

            return textPositions;
        }

        public async Task<string> ExtractText()
        {
            if (this.PdfPage == null)
                return String.Empty;

            var strategy = new SimpleTextExtractionStrategy();
            var pdfCanvasProcessor = new PdfCanvasProcessor(strategy);

            pdfCanvasProcessor.ProcessPageContent(PdfPage);
            string result = strategy.GetResultantText();

            return result; 
        }

        public void Dispose()
        {
            this.FileProcessor = null;
            this.PdfPage = null;
        }

        public override Task FindTextAsync(string textToFind, RedactorEnums.RedactionMatchingStrategy findType = RedactorEnums.RedactionMatchingStrategy.FixedPhrase, Action<List<PageTextPosition>>? onCompleted = null)
        {
            return Task.Run(async () =>
            {
                DateTime startDateTime = DateTime.Now;
                DateTime endDateTime = DateTime.Now;
                List<PageTextPosition> results = await this.FindText(textToFind, findType);
                if (onCompleted != null)
                    onCompleted(results);

                endDateTime = DateTime.Now;
                TimeSpan elapseTime = endDateTime - startDateTime;
                Console.WriteLine(@$"Text:Worker Completed => Start:{startDateTime.ToString()}|End:{endDateTime.ToString()}|Duration:{elapseTime}");
            });
        }
    }

}