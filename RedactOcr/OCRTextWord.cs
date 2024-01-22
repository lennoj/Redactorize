namespace Redactorize.RedactOcr
{
    public class OCRTextWord
    {
        public OCRTextLine OCRTextLine { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string TextValue { get; set; } = String.Empty;

        public List<OCRTextChar> Chars { get; set; }

        public OCRTextWord(int x, int y, int width, int height, string textValue , OCRTextLine ocrTextLine)
        { 
            this.X = x; 
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.TextValue = textValue;
            this.OCRTextLine = ocrTextLine;
            this.Chars = new List<OCRTextChar>();   
        }
    }
}
