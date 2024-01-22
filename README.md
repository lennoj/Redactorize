# Redactorize
PDF Redaction Library using iText7, Tesseract and SkiaSharp

## Usage
### Obtain Text position and dimension
`SampleGetTextPosition.cs`
```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redactorize;
using Redactorize.Redact;

namespace SampleApp.Redactorize.UnitTesting
{
    
    internal class SampleGetTextPosition
    {
        public void Start()
        {
            string inputFilePath = @"C:\dev\prototypes\SampleApp.Redactorize\Samples\SampleGetTextPosition.pdf";
            string outputFilePath = @"C:\dev\prototypes\SampleApp.Redactorize\Samples\SampleGetTextPosition.pdf";
            Redactor.Initialize(@"C:\dev\prototypes\SampleApp.Redactorize\Samples");

            RedactionParameter p1 = new RedactionParameter(RedactorEnums.RedactionMatchingStrategy.FixedPhrase, @"5163");
            RedactionParameter p2 = new RedactionParameter(RedactorEnums.RedactionMatchingStrategy.FixedPhrase, @"2125");
            RedactionParameter p3 = new RedactionParameter(RedactorEnums.RedactionMatchingStrategy.FixedPhrase, @"0659");
            RedactionParameter p4 = new RedactionParameter(RedactorEnums.RedactionMatchingStrategy.FixedPhrase, @"7817");

            List<PageTextPosition> results = Redactor.GetMatchPageTextPosition(inputFilePath,outputFilePath , RedactorEnums.RedactionType.TextAndImage, p1, p2,p3, p4);
        }
    }
}
```

`Program.cs`
```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleApp.Redactorize.UnitTesting;

namespace SampleApp.Redactorize
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            SampleGetTextPosition sampleGetTextPosition = new SampleGetTextPosition();
            sampleGetTextPosition.Start();
        }
    }
}
```

### Redact
`SampleRedact.cs`
```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redactorize;
using Redactorize.Redact;

namespace SampleApp.Redactorize.UnitTesting
{
    
    internal class SampleRedact
    {
        public void Start()
        {
            string inputFilePath = @"C:\dev\prototypes\SampleApp.Redactorize\Samples\SampleRedact.pdf";
            string outputFilePath = @"C:\dev\prototypes\SampleApp.Redactorize\Samples\SampleRedact.pdf";
            Redactor.Initialize(@"C:\dev\prototypes\SampleApp.Redactorize\Samples");

            RedactionParameter p1 = new RedactionParameter(RedactorEnums.RedactionMatchingStrategy.FixedPhrase, @"5163");
            RedactionParameter p2 = new RedactionParameter(RedactorEnums.RedactionMatchingStrategy.FixedPhrase, @"2125");
            RedactionParameter p3 = new RedactionParameter(RedactorEnums.RedactionMatchingStrategy.FixedPhrase, @"0659");
            RedactionParameter p4 = new RedactionParameter(RedactorEnums.RedactionMatchingStrategy.FixedPhrase, @"7817");

            Redactor.Process(inputFilePath,outputFilePath , RedactorEnums.RedactionType.TextAndImage, p1, p2,p3, p4);
        }
    }
}
```

`Program.cs`
```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleApp.Redactorize.UnitTesting;

namespace SampleApp.Redactorize
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            SampleRedact sampleRedact = new SampleRedact();
            sampleRedact.Start();
        }
    }
}
```

### Redact using Regular Expression
`SampleRedactRegEx.cs`
```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redactorize;
using Redactorize.Redact;

namespace SampleApp.Redactorize.UnitTesting
{
    
    internal class SampleRedactRegEx
    {
        public void Start()
        {
            string inputFilePath = @"C:\dev\prototypes\SampleApp.Redactorize\Samples\SampleRedactRegEx.pdf";
            string outputFilePath = @"C:\dev\prototypes\SampleApp.Redactorize\Samples\SampleRedactRegEx.pdf";
            Redactor.Initialize(@"C:\dev\prototypes\SampleApp.Redactorize\Samples");

            // Searching for Pattern 123A 123A 23A 12
            RedactionParameter p1 = new RedactionParameter(RedactorEnums.RedactionMatchingStrategy.RegularExpression, @"(?:(\d{2,3}[A]){1,}(\d{2,3}))");
            Redactor.Process(inputFilePath,outputFilePath , RedactorEnums.RedactionType.TextAndImage, p1, p2,p3, p4);
        }
    }
}
```

## Please make sure to follow and visit
* [PDF Manipulation | iText7](https://github.com/itext/itext7-dotnet)
* [PDF Redaction | PdfSweep - iText7 Add-On for PDF Redaction](https://github.com/itext/i7n-pdfsweep)
* [Image Text Extraction | Tesseract OCR](https://github.com/tesseract-ocr/tessdoc)
* [Image Processing | SkiaSharp](https://github.com/mono/SkiaSharp)

