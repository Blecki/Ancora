using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ancora.Parsers
{
    public class OneOrMany : Parser
    {
        private Parser SubParser;

        public OneOrMany(Parser SubParser)
        {
            this.SubParser = SubParser;
        }

        public override Parser Clone()
        {
            return new OneOrMany(SubParser);
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
                        ParseSucceeded = r.Children.Count > 0,
                        Node = passThroughChild == null ? r : passThroughChild,
                        StreamState = InputStream,
                        Flags = Flags
                    };
            }
        }
    }
}
