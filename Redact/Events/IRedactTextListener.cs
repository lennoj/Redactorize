using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;

namespace Redactorize.Redact.Events
{
    public interface IRedactTextListener
    {
        bool OnTextLineRead(Rectangle Rectangle, string TextValue, int PageNumber);
        bool OnTextMatchFound(Rectangle Rectangle, string TextValue, int PageNumber);
        bool OnRedact(PageTextPosition PageTextPosition);
    }
}
