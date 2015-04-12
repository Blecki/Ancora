using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ancora.Parsers
{
    public class Token : Parser
    {
        private Func<char, bool> IsLegalCharacter;

        public Token(Func<char,bool> IsLegalCharacter)
        {
            this.IsLegalCharacter = IsLegalCharacter;
        }

        public override Parser Clone()
        {
            return new Token(IsLegalCharacter);
        }

        public override ParseResult Parse(StringIterator InputStream)
        {
            var text = "";
            while (!InputStream.AtEnd && IsLegalCharacter(InputStream.Next))
            {
                text += InputStream.Next;
                InputStream = InputStream.Advance();
            }

            return new ParseResult
            {
                ParseSucceeded = !String.IsNullOrEmpty(text),
                Node = new AstNode
                {
                    NodeType = String.IsNullOrEmpty(AstNodeType) ? "UNNAMED" : AstNodeType,
                    Value = text
                },
                StreamState = InputStream,
                Flags = Flags
            };
        }
    }
}
