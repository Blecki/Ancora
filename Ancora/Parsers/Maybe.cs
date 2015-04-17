using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ancora.Parsers
{
    public class Maybe : Parser
    {
        private Parser SubParser;

        public Maybe(Parser SubParser)
        {
            this.SubParser = SubParser;
        }

        protected override Parser ImplementClone()
        {
            return new Maybe(SubParser);
        }

        public override ParseResult Parse(StringIterator InputStream)
        {
            var subResult = SubParser.Parse(InputStream);
            if (subResult.ParseSucceeded) return subResult.ApplyFlags(Flags);
            return new ParseResult
            {
                ParseSucceeded = true,
                Node = null,
                StreamState = InputStream,
                Flags = Flags
            };
        }
    }
}
