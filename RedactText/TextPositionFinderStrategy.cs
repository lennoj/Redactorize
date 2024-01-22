﻿using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf;
using Redactorize.Redact;

namespace Redactorize.RedactText
{
    public class TextPositionFinderStrategy : LocationTextExtractionStrategy
    {
        public readonly string searchText = String.Empty;
        private Matrix? textMatrix;
        private List<Rectangle> Rectangle { get; set; }

        public TextPositionFinderStrategy(string searchText)
        {
            this.searchText = searchText ?? String.Empty;
            this.Rectangle = new List<Rectangle>();
        }

        public override void EventOccurred(IEventData data, EventType type)
        {
            if (type == EventType.RENDER_TEXT)
            {
                Rectangle? foundRect = null;
                TextRenderInfo renderInfo = (TextRenderInfo)data;
                string text = renderInfo.GetText();

                if (text.Contains(searchText) && searchText != String.Empty)
                {
     
                    int indexStart = text.IndexOf(searchText);
                    textMatrix = renderInfo.GetTextMatrix();                    
                    foundRect = renderInfo.GetBaseline().GetBoundingRectangle();
                    string previousText = text.Substring(0, indexStart);
                    // get the previous text width
                    float previousTextWidth = renderInfo.GetFont().GetWidth(previousText, renderInfo.GetFontSize());
                    // get the found text width
                    float foundTextWidth = renderInfo.GetFont().GetWidth(searchText, renderInfo.GetFontSize());
                    // get the actual position of the found text (X-axis)
                    float foundTextX = foundRect.GetX() + previousTextWidth;
                    foundRect.SetWidth(foundTextWidth);
                    foundRect.SetHeight((float)Math.Floor(renderInfo.GetFontSize()));
                    foundRect.SetX(foundTextX);
                    this.Rectangle.Add(foundRect); 
                    return;
                }
                else if(searchText == String.Empty)
                {
                    textMatrix = renderInfo.GetTextMatrix();
                    foundRect = renderInfo.GetBaseline().GetBoundingRectangle();
                    this.Rectangle.Add(foundRect);
                    return;
                }
            }
        }

        public override string GetResultantText()
        {
            return searchText;
        }

        public List<Rectangle> GetResult()
        {
            return this.Rectangle;
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
