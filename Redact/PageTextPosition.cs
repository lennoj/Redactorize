namespace Redactorize.Redact
{
    public class PageTextPosition
    {
   
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;

        public int PageIndex { get; set; } = 0;

        public string TextValue { get; set; } = string.Empty;

        public RedactorEnums.PageTextSource Source { get; set; }

        public PageTextPosition()
        {

        }

        public PageTextPosition(int pageIndex,int x, int y, int width, int height , string textValue , RedactorEnums.PageTextSource textSoure)
        {
            this.PageIndex = pageIndex;
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.TextValue = textValue;
            this.Source = textSoure;
        }

    }
}
