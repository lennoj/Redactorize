using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;

namespace Redactorize.Redact.Events
{
    public interface IRedactImageListener
    {
        bool OnImageTextLineRead(Rectangle Rectangle, string TextValue, int PageNumber);
        bool OnImageTextMatchFound(Rectangle Rectangle, string TextValue, int PageNumber);
        bool OnImageTextWordRead(Rectangle Rectangle, string TextValue, int PageNumber);
        bool OnImageRedact(PageTextPosition PageTextPosition);
    }
}
