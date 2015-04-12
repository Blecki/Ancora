using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ancora.Parsers
{
    public class KeyWord : Parser
    {
        private String Word;

        public KeyWord(String Word)
        {
            this.Word = Word;
        }

        public override Parser Clone()
        {
            return new KeyWord(Word);
        }

        public override ParseResult Parse(StringIterator InputStream)
        {
            var text = InputStream.Peek(Word.Length);
            if (text == Word)
                return new ParseResult
                {
                    ParseSucceeded = true,
                    Node = ShouldCreateAst ? new AstNode
                    {
                        NodeType = AstNodeType,
                        Value = Word
                    } : null,
                    StreamState = InputStream.Advance(Word.Length),
                    Flags = Flags
                };
            else
                return ParseResult.Failure;
        }
    }
}
