using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ancora.Parsers
{
    public class Character : Parser
    {
        private char Char;

        public Character(char Char)
        {
            this.Char = Char;
        }

        protected override Parser ImplementClone()
        {
            return new Character(Char);
        }

        public override ParseResult Parse(StringIterator InputStream)
        {
            if (InputStream.AtEnd || InputStream.Next != Char)
                return ParseResult.Failure;
            return new ParseResult 
            { 
                ParseSucceeded = true, 
                Node = ((Flags & ParserFlags.CREATE_AST) == ParserFlags.CREATE_AST) ? new AstNode { NodeType = AstNodeType, Value = Char } : null,
                StreamState = InputStream.Advance(),
                Flags = Flags
            };
        }
    }
}
