using iText.Kernel.Pdf;
using Redactorize.RedactOcr;

namespace Redactorize.Redact
{
    internal class PageImageProcessor : BasePageProcessor, IDisposable
    {

        public PageImageProcessor(FileProcessor fileProcessor, PdfPage pdfPage, int pageIndex) : base(fileProcessor, pdfPage, pageIndex)
        {

        }

        public override async Task<List<PageTextPosition>> FindText(string textToFind, RedactorEnums.RedactionMatchingStrategy findType = RedactorEnums.RedactionMatchingStrategy.FixedPhrase)
        {
            List<PageTextPosition> textPositions = new List<PageTextPosition>();
            List<string> matches = new List<string>();
            if (this.PdfPage == null)
                return textPositions;

            // at this moment, all image(s) text(s) is extracted
            List<PageTextPosition> textImages = TextFromImagePositionFinderStrategy.GetTextPosition(textToFind, this.PdfPage, this.PdfPageIndex);

            // if the text of the image matches the fixedPhrase or RegEx Pattern, return the position and dimension to redact
            foreach (PageTextPosition textImage in textImages) {
                if (findType == RedactorEnums.RedactionMatchingStrategy.FixedPhrase)
                {
                    matches = BasePageProcessor.FindTextUsingFixedPhrase(textImage.TextValue, textToFind);
                }
                else if (findType == RedactorEnums.RedactionMatchingStrategy.RegularExpression)
                {
                    matches = BasePageProcessor.FindTextUsingRegEx(textImage.TextValue, textToFind);
                }

                if(matches.Count > 0)
                {
                    textPositions.Add(textImage);
                }
            }

            return textPositions;
        }

    }
}
