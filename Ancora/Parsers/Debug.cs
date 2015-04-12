using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ancora.Parsers
{
    public class Debug : Parser
    {
        private Action<StringIterator> CallOnParse;

        public Debug(Action<StringIterator> CallOnParse)
        {
            this.CallOnParse = CallOnParse;
        }

        public override Parser Clone()
        {
            return new Debug(CallOnParse);
        }

        public override ParseResult Parse(StringIterator InputStream)
        {
            CallOnParse(InputStream);
            return new ParseResult { ParseSucceeded = true, StreamState = InputStream };
        }
    }
}
