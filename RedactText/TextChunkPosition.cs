using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace Redactorize.RedactText
{
    internal class TextChunkPosition
    {
        public int @Index { get; set; } = 0;
        public string @String { get; set; } = String.Empty;
        public Rectangle @Rectangle { get; set; } 

        public TextChunkPosition(string @String, Rectangle @Rectangle, int @Index)
        {
            this.String = @String;
            this.Rectangle = @Rectangle;
            this.Index = @Index;
        }

        public static TextChunkPosition? Merge(List<TextChunkPosition> Chunks)
        {
            StringBuilder stringBuilder = new StringBuilder();
            TextChunkPosition? wholeWordChunk = null;
            foreach (TextChunkPosition chunk in Chunks) 
            {
                stringBuilder.Append(chunk.String); 
            }

            TextChunkPosition? firstChunk = Chunks.First();
            TextChunkPosition? lastChunk = Chunks.Last();

            if (firstChunk != null && lastChunk != null)
            {
                float x = firstChunk.Rectangle.GetX();
                float y = firstChunk.Rectangle.GetY();
                float w = lastChunk.Rectangle.GetX() - x + lastChunk.Rectangle.GetWidth();
                float h = lastChunk.Rectangle.GetY() - y + lastChunk.Rectangle.GetHeight();
                iText.Kernel.Geom.Rectangle r = new iText.Kernel.Geom.Rectangle(x, y, w, h);
                wholeWordChunk = new TextChunkPosition(stringBuilder.ToString(), r, firstChunk.Index);
            }

            return wholeWordChunk;
        }
    }
}
