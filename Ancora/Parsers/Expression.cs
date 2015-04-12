using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ancora.Parsers
{
    public class Expression : Parser
    {
        private Parser TermParser;
        private Parser OperatorParser;
        private OperatorTable OperatorTable;

        public Expression(Parser TermParser, Parser OperatorParser, OperatorTable OperatorTable)
        {
            this.TermParser = TermParser;
            this.OperatorParser = OperatorParser;
            this.OperatorTable = OperatorTable;

            //if (!this.OperatorParser.ShouldCreateAst) throw new InvalidOperationException("Expression requires operator parser to produce an ast node.");
            //if (!this.TermParser.ShouldCreateAst) throw new InvalidOperationException("Expression requires term parser to produce an ast node.");
        }

        public override Parser Clone()
        {
            return new Expression(TermParser, OperatorParser, OperatorTable);
        }

        public override ParseResult Parse(StringIterator InputStream)
        {
            var firstLhs = TermParser.Parse(InputStream);
            if (!firstLhs.ParseSucceeded || firstLhs.Node == null) return ParseResult.Failure;

            return ParseExpression(firstLhs.Node, firstLhs.StreamState, 0);
        }

        private ParseResult ParseExpression(AstNode LHS, StringIterator InputStream, int MinimumPrecedence)
        {
            while (true)
            {
                var opParser = OperatorParser.Parse(InputStream);
                if (!opParser.ParseSucceeded)
                    return new ParseResult { Node = LHS, ParseSucceeded = true, StreamState = InputStream };

                var precedence = OperatorTable.FindPrecedence(opParser.Node.Value.ToString());
                if (precedence < MinimumPrecedence) return new ParseResult { Node = LHS, ParseSucceeded = true, StreamState = InputStream };

                var op = opParser.Node.Value.ToString();
                InputStream = opParser.StreamState;
                var rhsParse = TermParser.Parse(InputStream);

                if (!rhsParse.ParseSucceeded) return new ParseResult { ParseSucceeded = false };

                while (true)
                {
                    opParser = OperatorParser.Parse(rhsParse.StreamState);
                    if (opParser.ParseSucceeded)
                    {
                        var nextPrecedence = OperatorTable.FindPrecedence(opParser.Node.Value.ToString());
                        if (nextPrecedence > precedence)
                            rhsParse = ParseExpression(rhsParse.Node, rhsParse.StreamState, nextPrecedence);
                        else
                            break;
                    }
                    else
                        break;
                }

                InputStream = rhsParse.StreamState;

                var r = new AstNode
                {
                    NodeType = "BINARYOP",
                    Value = op
                };

                r.Children.Add(LHS);
                r.Children.Add(rhsParse.Node);

                LHS = r;
            }
        }
    }
}
