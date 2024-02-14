using SkiaSharp;
using TesseractOCR.Enums;
using TesseractOCR;
using iText.Kernel.Pdf.Xobject;
using TesseractOCR.Layout;
using Redactorize.Redact;

namespace Redactorize.RedactOcr
{
    internal class ImageTextReader
    {
        public static void GetTextFromImage(ImageInfo imageInfo)
        {
            // Convert SKImage to a PNG byte array
            byte[] pngData;
            using (var stream = new MemoryStream())
            {
                imageInfo.Image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
                pngData = stream.ToArray();
            }

            imageInfo.TextPosition = new List<PageTextPosition>();

            // Convert the PNG byte array to a Pix image
            using (SKBitmap bitmap = SKBitmap.Decode(pngData))
            using (Engine tesseractEngine = new Engine(Redactor.TesseractOCRDataPath, "eng", EngineMode.Default))
            using (TesseractOCR.Pix.Image pix = TesseractOCR.Pix.Image.LoadFromMemory(pngData))
            using (Page tesseractPage = tesseractEngine.Process(pix))
            {
                foreach (Block block in tesseractPage.Layout)
                    if (block.Text != null)
                    {
                        float offsetX = 0;
                        float offsetY = 0;

                        if (block.BoundingBox != null)
                        {
                            offsetX = block.BoundingBox.Value.X1;
                            offsetY = block.BoundingBox.Value.Y1;
                        }

                        GetBlockText(block, imageInfo);
                    }
                }
            }

        private static void GetBlockText(Block block, ImageInfo imageInfo)
        {
            
            foreach (TesseractOCR.Layout.Paragraph paragraph in block.Paragraphs)
            {
                if (paragraph.Text != null && paragraph.BoundingBox != null)
                    ExtractTextLines(paragraph, imageInfo);

            }
        }

        private static void ExtractTextLines(TesseractOCR.Layout.Paragraph paragraph , ImageInfo imageInfo)
        {
            foreach (TextLine textLine in paragraph.TextLines)
            {
                if(textLine.Text != null && textLine.BoundingBox != null)
                {
                    Rect rectLine = textLine.BoundingBox.Value;
                    float x = rectLine.X1;
                    float y = rectLine.Y1;

                    OCRTextLine oCRTextLine = new OCRTextLine((int)x, (int)y, rectLine.Width, rectLine.Height, textLine.Text);
                    ExtractWords(textLine, oCRTextLine);
                    imageInfo.OCRTextLines.Add(oCRTextLine);
                }
                    
            }
        }
        private static void ExtractWords(TextLine textLine, OCRTextLine oCRTextLine)
        {
            foreach (Word word in textLine.Words)
            {
                if (word.Text != null && word.BoundingBox != null)
                {
                    Rect rectLine = word.BoundingBox.Value;
                    float x = rectLine.X1;
                    float y = rectLine.Y1;
                    int fontSize = word.FontProperties.PointSize;
                    int newWidth = fontSize * word.Text.Length;
                    OCRTextWord oCRWord = new OCRTextWord((int)x, (int)y, newWidth, rectLine.Height, word.Text, oCRTextLine);
                    //ExtractChars(word, oCRWord);
                    
                    oCRTextLine.Words.Add(oCRWord);
                }
            }
        }

        public static SKBitmap ConvertToSKBitmap(PdfImageXObject imageXObject)
        {
            // Extract the raw image data from the PdfImageXObject
            byte[] imageData = imageXObject.GetImageBytes();

            // Use SkiaSharp to decode the image data into an SKBitmap
            using (var stream = new SKMemoryStream(imageData))
            {
                return SKBitmap.Decode(stream);
            }
        }
    }
}
