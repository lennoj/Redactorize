using Redactorize.Redact;
using iText.Kernel.Pdf;

namespace Redactorize.Redact
{
    public class FileProcessor : IDisposable
    {
        public string FilePath { get; set; } = string.Empty;
        public List<BasePageProcessor>? PageProcessors { get; set; } = null;

        public PdfReader? PdfReaderInstance { get; set; } = null;
        public PdfDocument? PdfDocumentInstance { get; set; } = null;

        public RedactorEnums.RedactionType RedactionType { get; set; }
    
        public FileProcessor(string filePath, RedactorEnums.RedactionType redactionType = RedactorEnums.RedactionType.TextAndImage) 
        {
            this.PageProcessors = new List<BasePageProcessor>();
            this.FilePath = filePath;
            this.RedactionType = redactionType;
        }

        public void Open()
        {
            if (!File.Exists(FilePath))
                throw new Exception($"File '{this.FilePath}' doesn't exist");

            this.PdfReaderInstance = new PdfReader(this.FilePath);
            this.PdfDocumentInstance = new PdfDocument(this.PdfReaderInstance);
            
            for (int pageIndex = 0; pageIndex < PdfDocumentInstance.GetNumberOfPages(); pageIndex++)
            {
                if(this.RedactionType == RedactorEnums.RedactionType.TextOnly)
                {
                    this.PageProcessors?.Add(new PageTextProcessor(this, PdfDocumentInstance.GetPage(pageIndex + 1), pageIndex));
                }
                else if(this.RedactionType == RedactorEnums.RedactionType.ImageOnly) 
                {
                    this.PageProcessors?.Add(new PageImageProcessor(this, PdfDocumentInstance.GetPage(pageIndex + 1), pageIndex));
                }
                else if(this.RedactionType == RedactorEnums.RedactionType.TextAndImage)
                {
                    this.PageProcessors?.Add(new PageTextProcessor(this, PdfDocumentInstance.GetPage(pageIndex + 1), pageIndex));
                    this.PageProcessors?.Add(new PageImageProcessor(this, PdfDocumentInstance.GetPage(pageIndex + 1), pageIndex));
                }
            }
        }

        public async Task<List<PageTextPosition>> FindText(string textToFind , RedactorEnums.RedactionMatchingStrategy findType = RedactorEnums.RedactionMatchingStrategy.FixedPhrase)
        {
            List<PageTextPosition> returnList = new List<PageTextPosition>();
            List<Task<List<PageTextPosition>>> tasks = new List<Task<List<PageTextPosition>>>();
            if (this.PageProcessors == null)
                throw new Exception("No PDF file was loaded yet.");

            foreach (BasePageProcessor pageProcessor in this.PageProcessors)
            {
                tasks.Add(pageProcessor.FindText(textToFind, findType));
            }

            List<PageTextPosition>[] results = await Task.WhenAll(tasks);
            foreach (List<PageTextPosition> taskResult in results)
            {
                returnList.AddRange(taskResult);
            }

            return returnList;
        }

        public async Task<List<string>> ExtractText()
        {
            List<string> returnList = new List<string>();
            List<Task<string>> tasks = new List<Task<string>>();
            if (this.PageProcessors == null)
                throw new Exception("No PDF file was loaded yet.");

            foreach (PageTextProcessor pageProcessor in this.PageProcessors)
            {
                tasks.Add(pageProcessor.ExtractText());
            }

            string[] results = await Task.WhenAll(tasks);
            foreach (string taskResult in results)
            {
                returnList.Add(taskResult);
            }

            return returnList;
        }


        public void Dispose()
        {
           if(this.PageProcessors != null)
            {
                lock (this.PageProcessors)
                {
                    foreach (BasePageProcessor processor in this.PageProcessors) { processor.Dispose(); }
                    this.PageProcessors?.Clear();
                    this.PageProcessors = null;
                }
            }

            this.PdfDocumentInstance = null ;
            this.PdfReaderInstance = null;
        }
    }
}
