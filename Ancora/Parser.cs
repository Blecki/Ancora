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
        IGNORE_LEADING_WHITESPACE = 8,
        IGNORE_TRAILING_WHITESPACE = 16,
        CREATE_AST = 32,
    }

    public abstract class Parser
    {
        internal String AstNodeType = null;
        internal ParserFlags Flags = ParserFlags.NONE;

        #region Flag Modifiers

        public Parser CreateAst(String AstNodeType)
        {
            var r = this.Clone();
            r.Flags |= ParserFlags.CREATE_AST;
            r.AstNodeType = AstNodeType;
            return r;
        }

        public Parser Flatten()
        {
            var r = this.Clone();
            r.Flags |= ParserFlags.FLATTEN;
            return r;
        }

        public Parser Value()
        {
            var r = this.Clone();
            r.Flags |= ParserFlags.VALUE;
            return r;
        }

        public Parser PassThrough()
        {
            var r = this.Clone();
            r.Flags |= ParserFlags.PASSTHROUGH;
            return r;
        }

        #endregion

        protected Parser Clone()
        {
            var r = this.ImplementClone();
            r.Flags = Flags;
            r.AstNodeType = AstNodeType;
            return r;
        }

        public abstract ParseResult Parse(StringIterator InputStream);
        protected abstract Parser ImplementClone();

        #region Construction Ops

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

        #endregion

    }
}
