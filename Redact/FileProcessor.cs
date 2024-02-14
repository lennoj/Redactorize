using Redactorize.Redact;
using iText.Kernel.Pdf;
using TesseractOCR.Renderers;

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
            DateTime startDateTime = DateTime.Now;
            DateTime endDateTime = DateTime.Now;

            List<PageTextPosition> returnList = new List<PageTextPosition>();
            List<Task> tasks = new List<Task>();
            if (this.PageProcessors == null)
                throw new Exception("No PDF file was loaded yet.");

            Action<List<PageTextPosition>> onCompleteCallBack = (List<PageTextPosition> result) =>
            {
                returnList.AddRange(result);
            };

            /*
            foreach (BasePageProcessor pageProcessor in this.PageProcessors)
            {
                if(pageProcessor.GetType() == typeof(PageTextProcessor))
                {
                    Task task = pageProcessor.FindTextAsync(textToFind, findType, onCompleteCallBack);
                    tasks.Add(task);
                }   
                else if(pageProcessor.GetType() == typeof(PageImageProcessor))
                {
                    Task task = pageProcessor.FindTextAsync(textToFind, findType, onCompleteCallBack);
                    tasks.Add(task);
                }                                
            }
            */

            foreach (BasePageProcessor pageProcessor in this.PageProcessors)
            {
                if (pageProcessor.GetType() == typeof(PageTextProcessor))
                {
                    List<PageTextPosition> result = pageProcessor.FindText(textToFind, findType).Result;
                    returnList.AddRange(result);
                }
                else if (pageProcessor.GetType() == typeof(PageImageProcessor))
                {
                    List<PageTextPosition> result = pageProcessor.FindText(textToFind, findType).Result;
                    returnList.AddRange(result);
                }
            }

            //await Task.WhenAll(tasks);
            //List<PageTextPosition>[] results = await Task.WhenAll(tasks);
            //foreach (List<PageTextPosition> taskResult in results)
            //{
            //    returnList.AddRange(taskResult);
            //}

            endDateTime = DateTime.Now;
            TimeSpan elapseTime = endDateTime - startDateTime;
            Console.WriteLine(@$"FileProcessor:FindText Completed => Start:{startDateTime.ToString()}|End:{endDateTime.ToString()}|Duration:{elapseTime}");
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
