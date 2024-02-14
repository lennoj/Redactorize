using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Redactorize.RedactText;
using TesseractOCR;

namespace Redactorize.Redact
{
    internal class TextChunkBuilder
    {
        private int @Index { get; set; }
        private List<TextChunkPosition>? TextChunks { get; }
        private TextChunkPosition? CurrentTextChunk { get; set; }
        private StringBuilder StringBuilder { get; set; }   
        public TextChunkBuilder() {
            this.TextChunks = new List<TextChunkPosition>();  
            this.StringBuilder = new StringBuilder();
            this.Index = -1;
        }

        public List<TextChunkPosition> Build(string SearchForString)
        {
            if (this.TextChunks == null)
                return new List<TextChunkPosition>();

            if (this.TextChunks.Count == 0)
                return new List<TextChunkPosition>();

            List<TextChunkPosition> foundChunks = new List<TextChunkPosition>();
            string textChunkString = this.StringBuilder.ToString();
            string regExPattern = $@"\b{Regex.Escape(SearchForString)}\b";
            Regex regExInstance = new Regex(regExPattern, RegexOptions.IgnoreCase);
            MatchCollection matches = regExInstance.Matches(textChunkString);

            if(matches.Count > 0)
            {
                /*
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        int startIndex = match.Index;
                        int endIndex = match.Index + match.Length;
                        List<TextChunkPosition> chunkPositions = this.TextChunks.Select(chunk => chunk)
                                                                                .Where(chunk => chunk.Index >= startIndex && chunk.Index <= endIndex)
                                                                                .ToList();
                        TextChunkPosition? foundChunk = TextChunkPosition.Merge(chunkPositions);
                        if (foundChunk != null)
                        {
                            foundChunks.Add(foundChunk);
                        }
                    }
                }
                */
                foreach (Match match in matches)
                {
                    foundChunks.Add(new TextChunkPosition(match.Value, new iText.Kernel.Geom.Rectangle(0, 0, 0, 0), match.Index));
                }
            }
          

            return foundChunks;
        }

        public bool TryBuild(string SearchForString)
        {
            if (this.TextChunks == null)
                return false;

            if (this.TextChunks.Count == 0)
                return false;

            List<TextChunkPosition> foundChunks = new List<TextChunkPosition>();
            string textChunkString = this.StringBuilder.ToString();
            string regExPattern = $@"\b{Regex.Escape(SearchForString)}\b";
            Regex regExInstance = new Regex(regExPattern, RegexOptions.IgnoreCase);
            MatchCollection matches = regExInstance.Matches(textChunkString);

            if (matches.Count > 0)
            {
                /*
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        int startIndex = match.Index;
                        int endIndex = match.Index + match.Length;
                        List<TextChunkPosition> chunkPositions = this.TextChunks.Select(chunk => chunk)
                                                                                .Where(chunk => chunk.Index >= startIndex && chunk.Index <= endIndex)
                                                                                .ToList();
                        TextChunkPosition? foundChunk = TextChunkPosition.Merge(chunkPositions);
                        if (foundChunk != null)
                        {
                            foundChunks.Add(foundChunk);
                        }
                    }
                }
                */
                foreach (Match match in matches)
                {
                    foundChunks.Add(new TextChunkPosition(match.Value, new iText.Kernel.Geom.Rectangle(0, 0, 0, 0), match.Index));
                }
            }

            return foundChunks.Count > 0;
        }

        public TextChunkPosition Add(string text, iText.Kernel.Geom.Rectangle rect, int index)
        {
            if (this.Index == -1)
                this.Index = 0;

            this.Index += text.Length;
            this.CurrentTextChunk = new TextChunkPosition(text, rect, index);
            this.StringBuilder.Append(text);
            this.TextChunks?.Add(this.CurrentTextChunk);
            return this.CurrentTextChunk;
        }

        public void Clear()
        {
            this.TextChunks?.Clear();
            this.CurrentTextChunk = null;
        }

        public TextChunkPosition? First()
        {
            if (this.TextChunks == null)
                return null;

            if (this.TextChunks.Count == 0)
                return null;

            return this.TextChunks.First();
        }

        public TextChunkPosition? Last()
        {
            if (this.TextChunks == null)
                return null;

            if (this.TextChunks.Count == 0)
                return null;

            return this.TextChunks.Last();
        }

        public TextChunkPosition? Current()
        {
            if (this.TextChunks == null)
                return null;

            if (this.TextChunks.Count == 0)
                return null;

            return this.CurrentTextChunk;
        }

    }
}
