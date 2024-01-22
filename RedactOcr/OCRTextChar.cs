namespace Redactorize.RedactOcr
{
    public class OCRTextChar
    {
        public OCRTextWord OCRTextWord { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string TextValue { get; set; } = String.Empty;

        public OCRTextChar(int x, int y, int width, int height, string textValue, OCRTextWord ocrTextWord)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.TextValue = textValue;
            this.OCRTextWord = ocrTextWord;
        }
    }
}
