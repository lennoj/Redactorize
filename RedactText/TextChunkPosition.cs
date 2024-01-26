using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;

namespace Redactorize.RedactText
{
    internal class TextChunkPosition
    {
        public string @String { get; set; } = String.Empty;
        public Rectangle @Rectangle { get; set; } 

        public TextChunkPosition(string @String, Rectangle @Rectangle)
        {
            this.String = @String;
            this.Rectangle = @Rectangle;
        }
    }
}
