using iText.Kernel.Pdf;
using iText.Layout;
using iText.Kernel.Geom;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using iText.PdfCleanup.Autosweep;
using static Redactorize.Redact.RedactorEnums;
using Redactorize.Redact.Events;
using iText.PdfCleanup;

namespace Redactorize.Redact
{
    public class RedactorCore
    {
        public static void Redact(FileProcessor fileProcessor, 
            List<PageTextPosition> textPositions, 
            string outputFilePath , 
            float scaleFactorX = 1.0f, 
            float scaleFactorY = 1.0f)
        {
            PdfReader? reader = new PdfReader(fileProcessor.FilePath);
            PdfWriter? writer = new PdfWriter(outputFilePath);
            reader.SetUnethicalReading(true);

            PdfDocument? pdf = new PdfDocument(reader, writer);
            Document? document = new Document(pdf);

            foreach (PageTextPosition textPosition in textPositions)
            {
                int pageNumber = textPosition.PageIndex + 1; // specify the page number where you want to insert the rectangle

                if (textPosition.Source == RedactorEnums.PageTextSource.Text)
                {
                    ApplyRedactionToText(pageNumber, document, pdf, textPosition);
                }
                else if (textPosition.Source == RedactorEnums.PageTextSource.Image)
                {
                    ApplyRedactionToImage(pageNumber, document, pdf, textPosition);
                }
            }

            if(document != null)
                document.Close();

            if (pdf != null)
                pdf.Close();

            if(writer != null)
                writer.Close();

            if(reader != null)
                reader.Close();

            document = null;
            pdf = null;
            writer = null;
            reader = null;
        }

        public static void ApplyRedactionToText(int pageNumber, Document doc, PdfDocument pdfDoc, PageTextPosition textPosition, float scaleFactorX = 1.0f, float scaleFactorY = 1.0f)
        {
            //iText.PdfCleanup.Autosweep.RegexBasedCleanupStrategy strat = new RegexBasedCleanupStrategy("");
            iText.PdfCleanup.Autosweep.CompositeCleanupStrategy strategy = new CompositeCleanupStrategy();
            strategy.Add(new RegexBasedCleanupStrategy(textPosition.TextValue).SetRedactionColor(ColorConstants.BLACK));
            iText.PdfCleanup.PdfCleaner.AutoSweepCleanUp(pdfDoc, strategy);
        }

        //public static List<PageTextPosition> GetRedactionTextPositions(string regularExpressionString,  int pageNumber, Document doc, PdfDocument pdfDoc)
        //{
        //    List<PageTextPosition> pageTextPositions = new List<PageTextPosition>(); 
        //    //iText.PdfCleanup.Autosweep.RegexBasedCleanupStrategy strat = new RegexBasedCleanupStrategy("");
        //    iText.PdfCleanup.Autosweep.CompositeCleanupStrategy strategy = new CompositeCleanupStrategy();
        //    strategy.Add(new RegexBasedCleanupStrategy(regularExpressionString).SetRedactionColor(ColorConstants.BLACK));
        //    iText.PdfCleanup.Autosweep.PdfAutoSweepTools tentativeCleanup = new PdfAutoSweepTools(strategy);//(pdfDoc, strategy);
            
        //    IList<PdfCleanUpLocation> cleanupLocations = tentativeCleanup.GetPdfCleanUpLocations(pdfDoc);

        //    foreach (PdfCleanUpLocation location in cleanupLocations)
        //    {
        //        pageTextPositions.Add(new PageTextPosition(
        //                pageNumber, location.GetRegion().GetLeft(),
        //                location.GetRegion().GetRight(),
        //                location.GetRegion().GetWidth(),
        //                location.GetRegion().GetHeight(),
        //                location.
        //            ));
        //    }
        //}

        public static void ApplyRedactionToImage(int pageNumber, Document doc, PdfDocument pdfDoc, PageTextPosition textPosition, float scaleFactorX = 1.0f, float scaleFactorY = 1.0f)
        {
            PdfPage page = pdfDoc.GetPage(pageNumber);
            var canvas = new PdfCanvas(page);
            canvas.SetFillColor(ColorConstants.BLACK);
            canvas.Rectangle(textPosition.X, textPosition.Y, textPosition.Width * scaleFactorX, textPosition.Height * scaleFactorY);
            canvas.Fill();
        }       
    }
}
