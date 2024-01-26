using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Redactorize.RedactText;
using TesseractOCR;

namespace Redactorize.Redact
{
    internal class TextChunkBuilder
    {
        private List<TextChunkPosition>? TextChunks { get; }
        private TextChunkPosition? CurrentTextChunk { get; set; }

        public TextChunkBuilder() {
          this.TextChunks = new List<TextChunkPosition>();  
        }

        public List<TextChunkPosition> Build(string SearchForString)
        {
            if (this.TextChunks == null)
                return new List<TextChunkPosition>();

            if (this.TextChunks.Count == 0)
                return new List<TextChunkPosition>();

            List<TextChunkPosition> processedChunks = new List<TextChunkPosition>();
            List<TextChunkPosition> foundChunks = new List<TextChunkPosition>();
            List<TextChunkPosition> nextChunks = new List<TextChunkPosition>();
            List<TextChunkPosition> previousChunks = new List<TextChunkPosition>();
            StringBuilder tempBuilder = new StringBuilder();
            foreach (TextChunkPosition chunk in this.TextChunks)
            {
                // add the current chunk
                tempBuilder.Append(chunk.String);
                processedChunks.Add(chunk);

                string tempBuildString = tempBuilder.ToString();
                int markedIndex = tempBuildString.IndexOf(SearchForString);
                // check if the string we are searching is currently within the tempBuilder
                if (markedIndex > -1)
                {
                    tempBuilder.Clear();

                    // iterate each processed chunks
                    foreach (TextChunkPosition processedChunk in processedChunks)
                    {
                        tempBuilder.Append(processedChunk.String);
                        tempBuildString = tempBuilder.ToString();
                        if (tempBuildString.Length - 1 < markedIndex)
                        {
                            previousChunks.Add(processedChunk);
                        }
                        else
                        {
                            nextChunks.Add(processedChunk);
                        }
                    }

                    // Clear String Builder
                    tempBuilder.Clear();

                    // Build the nextChunk
                    foreach (TextChunkPosition nextChunk in nextChunks)
                    {
                        tempBuilder.Append(nextChunk.String);
                    }

                    tempBuildString = tempBuilder.ToString();

                    // If the SearchForString is equal to tempBuildString
                    // create a one whole chunk
                    if (tempBuildString.Equals(SearchForString))
                    {
                        TextChunkPosition? firstChunk = nextChunks.First();
                        TextChunkPosition? lastChunk = nextChunks.Last();


                        if (firstChunk != null && lastChunk != null)
                        {
                            float x = firstChunk.Rectangle.GetX();
                            float y = firstChunk.Rectangle.GetY();
                            float w = lastChunk.Rectangle.GetX() - x + lastChunk.Rectangle.GetWidth();
                            float h = lastChunk.Rectangle.GetY() - y + lastChunk.Rectangle.GetHeight();
                            iText.Kernel.Geom.Rectangle r = new iText.Kernel.Geom.Rectangle(x, y, w, h);

                            TextChunkPosition wholeWordChunk = new TextChunkPosition(tempBuildString, r);
                            foundChunks.Add(wholeWordChunk);
                        }
                    }

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

            List<TextChunkPosition> processedChunks = new List<TextChunkPosition>();
            List<TextChunkPosition> foundChunks = new List<TextChunkPosition>();
            List<TextChunkPosition> nextChunks = new List<TextChunkPosition>();
            List<TextChunkPosition> previousChunks =  new List<TextChunkPosition>();
            StringBuilder tempBuilder = new StringBuilder();
            foreach (TextChunkPosition chunk in this.TextChunks)
            {
                // add the current chunk
                tempBuilder.Append(chunk.String);
                processedChunks.Add(chunk);

                string tempBuildString = tempBuilder.ToString();
                int markedIndex = tempBuildString.IndexOf(SearchForString);
                // check if the string we are searching is currently within the tempBuilder
                if (markedIndex > -1)
                {
                    tempBuilder.Clear();
                    
                    // iterate each processed chunks
                    foreach (TextChunkPosition processedChunk in processedChunks)
                    {
                        tempBuilder.Append(processedChunk.String);
                        tempBuildString = tempBuilder.ToString();
                        if (tempBuildString.Length - 1  < markedIndex)
                        {
                            previousChunks.Add(processedChunk);
                        }
                        else
                        {
                            nextChunks.Add(processedChunk);
                        }                       
                    }

                    // Clear String Builder
                    tempBuilder.Clear();

                    // Build the nextChunk
                    foreach (TextChunkPosition nextChunk in nextChunks)
                    {
                        tempBuilder.Append(nextChunk.String);
                    }

                    tempBuildString = tempBuilder.ToString();

                    // If the SearchForString is equal to tempBuildString
                    // create a one whole chunk
                    if (tempBuildString.Equals(SearchForString))
                    {
                        TextChunkPosition? firstChunk = nextChunks.First();
                        TextChunkPosition? lastChunk = nextChunks.Last();

                        if(firstChunk != null && lastChunk != null)
                        {
                            float x = firstChunk.Rectangle.GetX();
                            float y = firstChunk.Rectangle.GetY();
                            float w = lastChunk.Rectangle.GetX() - x + lastChunk.Rectangle.GetWidth();
                            float h = lastChunk.Rectangle.GetY() - y + lastChunk.Rectangle.GetHeight();
                            iText.Kernel.Geom.Rectangle r = new iText.Kernel.Geom.Rectangle(x,y,w,h);

                            TextChunkPosition wholeWordChunk = new TextChunkPosition(tempBuildString, r);
                            foundChunks.Add(wholeWordChunk);
                        }

                   
                    }
                   
                }
               
            }

            return foundChunks.Count > 0;
        }

        public TextChunkPosition Add(string text, iText.Kernel.Geom.Rectangle rect)
        {
            this.CurrentTextChunk = new TextChunkPosition(text, rect);
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
