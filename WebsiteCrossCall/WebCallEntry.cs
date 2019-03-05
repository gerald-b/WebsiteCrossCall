using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteCrossCall
{
    class WebCallEntry
    {
        public String _line { get; private set; }
        public String _match {get; private set; }

        public WebCallEntry(String match, String line)
        {
            this._line = line;
            this._match = match;
        }
    }
}
