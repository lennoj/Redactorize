using System.Text;
using System.Text.RegularExpressions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using SkiaSharp;
using Redactorize.Redact;

namespace Redactorize.RedactOcr
{
    internal class TextFromImagePositionFinderStrategy : IEventListener
    {
        public readonly string searchText = String.Empty;
        
        private Matrix? textMatrix;
        private List<ImageInfo> ImagesInfo { get; set; }

        public TextFromImagePositionFinderStrategy()
        {
            this.searchText = searchText ?? String.Empty;
            this.ImagesInfo = new List<ImageInfo>();
        }

        public TextFromImagePositionFinderStrategy(string searchText)
        {
            this.searchText = searchText ?? String.Empty;
            this.ImagesInfo = new List<ImageInfo>();
        }

        public void EventOccurred(IEventData data, EventType type)
        {
            if (type != EventType.RENDER_IMAGE)
                return;

            ImageRenderInfo renderInfo = (ImageRenderInfo)data;
            PdfImageXObject imageObject = renderInfo.GetImage();
            Matrix imageMatrix = renderInfo.GetImageCtm();

            float DpiX = 1;
            float DpiY = 1;

            if (imageObject.GetPdfObject().GetAsNumber(PdfName.DCTDecode) != null)
            {
                 DpiX = imageObject.GetPdfObject().GetAsNumber(PdfName.DCTDecode).FloatValue();
                 DpiY = imageObject.GetPdfObject().GetAsNumber(PdfName.DCTDecode).FloatValue();
            }
            // Store information about the image
            var imageInfo = new ImageInfo
            {
                OCRTextLines = new List<OCRTextLine>(),
                Image = ImageTextReader.ConvertToSKBitmap(imageObject),
                Position = new SKPoint(imageMatrix.Get(6), imageMatrix.Get(7)),
                Dimensions = new SKSize(imageMatrix.Get(0), imageMatrix.Get(3)),
                DpiX = DpiX,
                DpiY = DpiY
            };

            // Extract all the textline and each words in in each line
            ImageTextReader.GetTextFromImage(imageInfo);

            // Apply Pre-RegEx filter if the pattern is RegEx
            if (isValidRegExPattern(searchText))
            {
                ApplyPreRegExFilter(searchText, imageInfo);
            }
            else
            {
                // check each line
                // check which line has match in the pattern
                if (searchText != String.Empty)
                {
                    imageInfo.OCRTextLines = (from OCRTextLine t in imageInfo.OCRTextLines
                                              where t.TextValue.Contains(searchText)
                                              select t).ToList();
                }

                // check each word in the line
                // if the series of word matches the pattern
                // calculate the x = (first-word x); width = (last-word x+width) 
                if (searchText != String.Empty)
                {
                    // only get 
                    foreach (OCRTextLine oCRTextLine in imageInfo.OCRTextLines)
                    {
                        oCRTextLine.Words = (from OCRTextWord word in oCRTextLine.Words
                                             where word.TextValue.Equals(searchText)
                                             select word).ToList();
                    }

                    // remove all TextLine that doesn't have any word
                    imageInfo.OCRTextLines = (from OCRTextLine t in imageInfo.OCRTextLines
                                              where t.Words.Count > 0
                                              select t).ToList();
                }
            }
            
            
            if(imageInfo.OCRTextLines.Count > 0) 
                ImagesInfo.Add(imageInfo);            

        }

        private bool isValidRegExPattern(string regExPattern)
        {
            regExPattern = regExPattern ?? String.Empty;
            if (regExPattern == String.Empty)
                return false;

            try
            {
                Regex.IsMatch("", regExPattern);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private void ApplyPreRegExFilter(string regExPattern, ImageInfo imageInfo)
        {
            Regex regex = new Regex(regExPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            
            // Get all only those textline that match the pattern
            List<OCRTextLine> filteredOCRTextLines = (from OCRTextLine t in imageInfo.OCRTextLines
                                                      where regex.Matches(t.TextValue).Count > 0
                                                      select t).ToList();

            // iterate each filtered OCR TextLine and get each matched set of word(s)
            foreach (OCRTextLine textLine in filteredOCRTextLines)
            {
                List<OCRTextWord> filteredOCRTextWords = new List<OCRTextWord>();
                OCRTextWord? textWord = null;
                int x = 0;
                int width = 0;
                int height = 0;
                string newTextValue = String.Empty;

                // get matches in the whole line
                MatchCollection textLineMatches = regex.Matches(textLine.TextValue);
                // iterate each matches and compare it to each words
                foreach (Match match in textLineMatches)
                {

                    List<OCRTextWord> foundFragments = (from OCRTextWord w in textLine.Words
                                where match.Value.Contains(w.TextValue)
                                select w).ToList();
                    
                    if(foundFragments.Count > 0)
                        filteredOCRTextWords.AddRange(foundFragments);
                }

                if(filteredOCRTextWords.Count > 0)
                {
                    textLine.Words = filteredOCRTextWords;

                    StringBuilder sb = new StringBuilder();
                    sb.AppendJoin("", (from OCRTextWord w in textLine.Words
                                       select w.TextValue).ToList());
                    newTextValue = sb.ToString();

                    // set the new dimension of the TextLine using the first:x of all words and width = last:x + last:width of all words
                    x = (from OCRTextWord t in textLine.Words
                         select t.X).First();
                    
                    width = (from OCRTextWord t in textLine.Words
                             select t.X).Last();

                    width += (from OCRTextWord t in textLine.Words
                             select t.Width).Last() - x;

                    height = (from OCRTextWord t in textLine.Words
                              select t.Height).First();

                    textLine.X = x;
                    textLine.Width = width;
                    textLine.TextValue = newTextValue;
                    //textLine.Height = height;
                }
            }

            imageInfo.OCRTextLines = filteredOCRTextLines;
        }

        public List<ImageInfo> GetResult()
        {
            return this.ImagesInfo;
        }

        

        public static List<PageTextPosition> GetTextPosition(string textToFind, PdfPage pdfPage , int pageIndex )
        {
            List<PageTextPosition> textPositions = new List<PageTextPosition>();
            var strategy = new TextFromImagePositionFinderStrategy(textToFind);
            var pdfCanvasProcessor = new PdfCanvasProcessor(strategy);

            pdfCanvasProcessor.ProcessPageContent(pdfPage);
            List<ImageInfo> imagesInfo = strategy.GetResult();

            foreach (ImageInfo imageInfo in imagesInfo)
            {
                foreach(OCRTextLine textLine in imageInfo.OCRTextLines)
                {
                    Console.WriteLine($"Text = {textLine.TextValue}");
                    textLine.Height = (int)imageInfo.FromImageToPDFDimensionY(pdfPage, textLine.Height);
                    textLine.X = (int)imageInfo.FromImageToPDFPositionX(pdfPage, textLine.X);
                    textLine.Y = (int)imageInfo.FromImageToPDFPositionY(pdfPage, textLine.Y, textLine.Height);
                    textLine.Width = (int)imageInfo.FromImageToPDFDimensionX(pdfPage, textLine.Width);
                    
                    textPositions.Add(new PageTextPosition(
                        pageIndex, 
                        textLine.X, 
                        textLine.Y, 
                        textLine.Width, 
                        textLine.Height, 
                        textLine.TextValue, 
                        RedactorEnums.PageTextSource.Image));
                }
            }

            return textPositions;
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return null;
        }
    }
}
