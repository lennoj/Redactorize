using Redactorize.Redact;

namespace Redactorize
{
    public class Redactor
    {
        public static string TesseractOCRDataPath { get; set; } = String.Empty;

        public static void Initialize(string tesseractOCRDataPath)
        {
            TesseractOCRDataPath = tesseractOCRDataPath; 
        }

        public static List<PageTextPosition> Process(
            string inputFilePath, 
            string outputFilePath, 
            RedactorEnums.RedactionType redactionType = RedactorEnums.RedactionType.TextAndImage, 
            params RedactionParameter[] strategies)
        {
            DateTime startDateTime = DateTime.Now;
            DateTime endDateTime = DateTime.Now;
            List<PageTextPosition> forRedactionTextPositions = new List<PageTextPosition>();
            FileProcessor fileProcessor = new FileProcessor(inputFilePath, redactionType);
            fileProcessor.Open();

            foreach (RedactionParameter strategy in strategies)
            {
                List<PageTextPosition>? resultPositions = null;
                resultPositions = fileProcessor.FindText(strategy.TextValue, strategy.Strategy).Result;
                forRedactionTextPositions.AddRange(resultPositions);
            }

            RedactorCore.Redact(fileProcessor, forRedactionTextPositions, outputFilePath, 1f, 1f);

            endDateTime = DateTime.Now;
            TimeSpan elapseTime = endDateTime - startDateTime;
            Console.WriteLine(@$"Redactor:Process Completed => Start:{startDateTime.ToString()}|End:{endDateTime.ToString()}|Duration:{elapseTime}");
            return forRedactionTextPositions;
        }

        public static List<PageTextPosition> GetMatchPageTextPosition(
            string inputFilePath,
            string outputFilePath,
            RedactorEnums.RedactionType redactionType = RedactorEnums.RedactionType.TextAndImage,
            params RedactionParameter[] strategies)
        {
            List<PageTextPosition> forRedactionTextPositions = new List<PageTextPosition>();
            FileProcessor fileProcessor = new FileProcessor(inputFilePath, redactionType);
            fileProcessor.Open();

            foreach (RedactionParameter strategy in strategies)
            {
                List<PageTextPosition>? resultPositions = null;
                resultPositions = fileProcessor.FindText(strategy.TextValue, strategy.Strategy).Result;
                forRedactionTextPositions.AddRange(resultPositions);
            }
           
            return forRedactionTextPositions;
        }

    }
}