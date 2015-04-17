using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ancora.Parsers
{
    public class NoneOrMany : Parser
    {
        private Parser SubParser;

        public NoneOrMany(Parser SubParser)
        {
            this.SubParser = SubParser;
        }

        protected override Parser ImplementClone()
        {
            return new NoneOrMany(SubParser);
        }

        public override ParseResult Parse(StringIterator InputStream)
        {
            var r = new AstNode { NodeType = String.IsNullOrEmpty(AstNodeType) ? "UNNAMED" : AstNodeType };
            AstNode passThroughChild = null;

            while (true)
            {
                var subResult = SubParser.Parse(InputStream);
                if (subResult.ParseSucceeded)
                {
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
                else
                    return new ParseResult
                    {
                        ParseSucceeded = true,
                        Node = passThroughChild == null ? r : passThroughChild,
                        StreamState = InputStream,
                        Flags = Flags
                    };
            }
        }
    }
}
