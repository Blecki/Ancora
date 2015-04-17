﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ancora.Parsers
{
    public class Sequence : Parser
    {
        private List<Parser> SubParsers;

        public Sequence(params Parser[] SubParsers)
        {
            this.SubParsers = new List<Parser>(SubParsers);
        }

        public static Sequence operator +(Sequence LHS, Parser RHS)
        {
            var r = LHS.Clone() as Sequence;
            r.SubParsers.Add(RHS);
            return r;
        }

        public static Sequence operator +(Sequence LHS, char RHS)
        {
            var r = LHS.Clone() as Sequence;
            r.SubParsers.Add(new Character(RHS));
            return r;
        }

        public static Sequence operator +(Sequence LHS, String RHS)
        {
            var r = LHS.Clone() as Sequence;
            r.SubParsers.Add(new KeyWord(RHS));
            return r;
        }

        protected override Parser ImplementClone()
        {
            return new Sequence(SubParsers.ToArray());
        }

        public override ParseResult Parse(StringIterator InputStream)
        {
            var r = new AstNode { NodeType = String.IsNullOrEmpty(AstNodeType) ? "UNNAMED" : AstNodeType };
            AstNode passThroughChild = null;
            foreach (var sub in SubParsers)
            {
                var subResult = sub.Parse(InputStream);
                if (!subResult.ParseSucceeded)
                    return ParseResult.Failure;
                InputStream = subResult.StreamState;

                if (subResult.Node != null)
                {
                    if (subResult.CheckFlag(ParserFlags.PASSTHROUGH) && passThroughChild == null)
                        passThroughChild = subResult.Node;
                    else if (subResult.CheckFlag(ParserFlags.FLATTEN))
                        r.Children.AddRange(subResult.Node.Children);
                    else if (subResult.CheckFlag(ParserFlags.VALUE))
                        r.Value = subResult.Node.Value;
                    else
                        r.Children.Add(subResult.Node);
                }
            }
            return new ParseResult
            {
                ParseSucceeded = true,
                StreamState = InputStream,
                Node = passThroughChild == null ? r : passThroughChild,
                Flags = Flags
            };
        }
    }
}
