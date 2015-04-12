using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ancora
{
    public struct ParseResult
    {
        public bool ParseSucceeded;
        public AstNode Node;
        public StringIterator StreamState;
        public ParserFlags Flags;

        public bool CheckFlag(ParserFlags Flag)
        {
            return (Flags & Flag) == Flag;
        }

        public static ParseResult Failure = new ParseResult { ParseSucceeded = false };

        public ParseResult ApplyFlags(ParserFlags Flags)
        {
            return new ParseResult
            {
                ParseSucceeded = ParseSucceeded,
                Node = Node,
                StreamState = StreamState,
                Flags = Flags
            };
        }
        
    }
}
