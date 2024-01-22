using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;

namespace Redactorize.Redact.Events
{
    public abstract class RedactionListener : IRedactImageListener, IRedactTextListener
    {
        protected RedactionListener() { }

        public abstract bool OnImageRedact(PageTextPosition PageTextPosition);
        public abstract bool OnImageTextLineRead(Rectangle Rectangle, string TextValue, int PageNumber);
        public abstract bool OnImageTextMatchFound(Rectangle Rectangle, string TextValue, int PageNumber);
        public abstract bool OnImageTextWordRead(Rectangle Rectangle, string TextValue, int PageNumber);
        public abstract bool OnRedact(PageTextPosition PageTextPosition);
        public abstract bool OnTextLineRead(Rectangle Rectangle, string TextValue, int PageNumber);
        public abstract bool OnTextMatchFound(Rectangle Rectangle, string TextValue, int PageNumber);
        public abstract bool OnTextWordRead(Rectangle Rectangle, string TextValue, int PageNumber);
    }
}
