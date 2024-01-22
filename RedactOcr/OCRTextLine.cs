namespace Redactorize.RedactOcr
{
    public class OCRTextLine
    {
        public List<OCRTextWord> Words { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string TextValue { get; set; } = String.Empty;

        public OCRTextLine(int x, int y, int width, int height, string textValue) 
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.TextValue = textValue;
            this.Words = new List<OCRTextWord>();
        }    
    }
}
