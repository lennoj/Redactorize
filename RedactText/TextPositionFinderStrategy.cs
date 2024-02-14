using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf;
using Redactorize.Redact;
using System.Text;
using System.Linq;

namespace Redactorize.RedactText
{
    public class TextPositionFinderStrategy : LocationTextExtractionStrategy
    {
        public readonly string searchText = String.Empty;
        private TextChunkBuilder @TextChunkBuilder { get; set; }
        private int @Index { get; set; }


        public TextPositionFinderStrategy(string searchText)
        {
            this.TextChunkBuilder = new TextChunkBuilder();
            this.searchText = searchText ?? String.Empty;
            this.Index = -1;
        }

        public override void EventOccurred(IEventData data, EventType type)
        {
            if (type == EventType.RENDER_TEXT)
            {
                Rectangle? foundRect = null;
                TextRenderInfo renderInfo = (TextRenderInfo)data;
                Matrix? textMatrix = renderInfo.GetTextMatrix();
                string text = renderInfo.GetText();                              
                foundRect = renderInfo.GetBaseline().GetBoundingRectangle();
                this.TextChunkBuilder.Add(text, foundRect, this.Index);
                this.Index += text.Length;
                foundRect.SetHeight((float)Math.Floor(renderInfo.GetFontSize()));
            }
        }

        public override string GetResultantText()
        {
            return base.GetResultantText();
        }

        
        public List<Rectangle> GetResult()
        {
            List<TextChunkPosition> foundWordChunks = this.TextChunkBuilder.Build(this.searchText);
            List<Rectangle> result = (List<Rectangle>)foundWordChunks.Select(chunk => chunk.Rectangle ).ToList();
            return result;
        }

        public static List<PageTextPosition> GetTextPosition(string textToFind, PdfPage pdfPage , int pageIndex )
        {
            List<PageTextPosition> textPositions = new List<PageTextPosition>();
            var strategy = new TextPositionFinderStrategy(textToFind);
            var pdfCanvasProcessor = new PdfCanvasProcessor(strategy);

            pdfCanvasProcessor.ProcessPageContent(pdfPage);
            List<Rectangle> rectangles = strategy.GetResult();

            foreach (Rectangle rect in rectangles)
            {
                PageTextPosition textPosition = new PageTextPosition(pageIndex, 
                    (int)rect.GetX(), 
                    (int)rect.GetY(), 
                    (int)rect.GetWidth(), 
                    (int)rect.GetHeight(), 
                    textToFind,
                    RedactorEnums.PageTextSource.Text);
                textPositions.Add(textPosition);
            }

            return textPositions;

        }
    }
}
