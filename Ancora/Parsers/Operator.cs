﻿using System;
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

        protected override Parser ImplementClone()
        {
            return new Operator(OperatorTable);
        }

        protected override ParseResult ImplementParse(StringIterator InputStream)
        {
            if (InputStream.AtEnd) return Fail("Unexpected end of stream");

            var opSoFar = "";
            while (true)
            {
                opSoFar += InputStream.Next;
                InputStream = InputStream.Advance();
                var possibleMatches = OperatorTable.PossibleMatches(opSoFar);
                if (possibleMatches == 0)
                    return Fail("Unable to match operator");
                else if (possibleMatches == 1 && OperatorTable.ExactMatches(opSoFar) == 1)
                    return new ParseResult
                    {
                        ParseSucceeded = true,
                        Node = (Flags & ParserFlags.CREATE_AST) == ParserFlags.CREATE_AST ? new AstNode
                        {
                            NodeType = AstNodeType,
                            Value = opSoFar
                        } : null,
                        StreamState = InputStream,
                        Flags = Flags
                    };

                if (InputStream.AtEnd) return Fail("Unexpected end of stream");
            }
        }
    }
}
