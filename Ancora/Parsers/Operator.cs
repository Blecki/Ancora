using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ancora.Parsers
{
    public class Operator : Parser
    {
        private OperatorTable OperatorTable;

        public Operator(OperatorTable OperatorTable)
        {
            this.OperatorTable = OperatorTable;
        }

        public override Parser Clone()
        {
            return new Operator(OperatorTable);
        }

        public override ParseResult Parse(StringIterator InputStream)
        {
            if (InputStream.AtEnd) return ParseResult.Failure;

            var opSoFar = "";
            while (true)
            {
                opSoFar += InputStream.Next;
                InputStream = InputStream.Advance();
                var possibleMatches = OperatorTable.PossibleMatches(opSoFar);
                if (possibleMatches == 0)
                    return ParseResult.Failure;
                else if (possibleMatches == 1 && OperatorTable.ExactMatches(opSoFar) == 1)
                    return new ParseResult
                    {
                        ParseSucceeded = true,
                        Node = ShouldCreateAst ? new AstNode
                        {
                            NodeType = AstNodeType,
                            Value = opSoFar
                        } : null,
                        StreamState = InputStream,
                        Flags = Flags
                    };

                if (InputStream.AtEnd) return ParseResult.Failure;
            }
        }
    }
}
