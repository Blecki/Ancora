using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ancora
{
    public enum ParserFlags
    {
        NONE = 0,
        PASSTHROUGH = 1,
        FLATTEN = 2,
        VALUE = 4,
    }

    public abstract class Parser
    {
        internal String AstNodeType = null;
        internal bool ShouldCreateAst = false;

        public Parser CreateAst(String AstNodeType)
        {
            this.ShouldCreateAst = true;
            this.AstNodeType = AstNodeType;
            return this;
        }

        internal ParserFlags Flags = ParserFlags.NONE;

        public Parser Flatten()
        {
            var r = this._Clone();
            r.Flags |= ParserFlags.FLATTEN;
            return r;
        }

        public Parser Value()
        {
            var r = this._Clone();
            r.Flags |= ParserFlags.VALUE;
            return r;
        }

        public Parser PassThrough()
        {
            var r = this._Clone();
            r.Flags |= ParserFlags.PASSTHROUGH;
            return r;
        }

        protected Parser _Clone()
        {
            var r = this.Clone();
            r.Flags = Flags;
            r.AstNodeType = AstNodeType;
            r.ShouldCreateAst = ShouldCreateAst;
            return r;
        }

        public abstract ParseResult Parse(StringIterator InputStream);
        public abstract Parser Clone();

        public static Parsers.Sequence operator +(Parser LHS, Parser RHS)
        {
            return new Parsers.Sequence(LHS, RHS);
        }

        public static Parsers.Sequence operator +(Parser LHS, char RHS)
        {
            return new Parsers.Sequence(LHS, new Parsers.Character(RHS));
        }

        public static Parsers.Sequence operator +(Parser LHS, String RHS)
        {
            return new Parsers.Sequence(LHS, new Parsers.KeyWord(RHS));
        }

        public static Parsers.Alternative operator |(Parser LHS, Parser RHS)
        {
            return new Parsers.Alternative(LHS, RHS);
        }

        public static Parser operator *(Parser LHS, Parser RHS)
        {
            return Grammar.DelimitedList(LHS, RHS);
        }

    }
}
