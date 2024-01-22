using iText.Kernel.Pdf;
using Redactorize.Redact;
using SkiaSharp;

namespace Redactorize.RedactOcr
{
    internal class ImageInfo
    {
        public SKPoint Position { get; set; }
        public SKSize Dimensions { get; set; }
        public SKBitmap Image { get; set; }
        public float DpiX { get; set; }
        public float DpiY { get; set; }
        public List<PageTextPosition> TextPosition { get; set; } 

        public List<OCRTextLine> OCRTextLines { get; set; }
        public ImageInfo() { }

        public ImageInfo(SKPoint position, SKSize dimensions, SKBitmap image, int dpiX, int dpiY , List<PageTextPosition> textPosition)
        {
            Position = position;
            Dimensions = dimensions;
            Image = image;
            TextPosition = textPosition;
            DpiX = dpiX; 
            DpiY = dpiY;
            this.OCRTextLines = new List<OCRTextLine>() ;
        }

        public float FromImageToPDFPositionX(PdfPage pdfPage,int x)
        {
            float ret =  ((float)pdfPage.GetPageSize().GetWidth()) * (x == 0 ? 0 : (float)x / ( (float)this.Image.Width ));
            return ret / DpiX;
        }

        public float FromImageToPDFPositionY(PdfPage pdfPage,int y,  int h)
        {
            float ret = pdfPage.GetPageSize().GetHeight() - ((float)pdfPage.GetPageSize().GetHeight()) * (y  == 0 ? 0 : ((float)y / (float)this.Image.Height ));
            ret -= h;
            return ret / this.DpiY;
        }

        public float FromImageToPDFDimensionX(PdfPage pdfPage, int w)
        {
            float ret = ((float)pdfPage.GetPageSize().GetWidth()) * ((float)w) / ((float)this.Image.Width);
            return ret / DpiX;
        }

        public float FromImageToPDFDimensionY(PdfPage pdfPage, int h)
        {
            float ret = ((float)pdfPage.GetPageSize().GetHeight()) * ((float)h) / ((float)this.Image.Height);
            return ret / this.DpiY;
        }
    }
}
