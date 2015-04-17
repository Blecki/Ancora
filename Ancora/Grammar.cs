using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ancora
{
    public class Grammar
    {
        public Parser Root = null;

        #region Parser Factory Functions

        public static Parsers.Sequence Sequence(params Parser[] SubParsers)
        {
            return new Parsers.Sequence(SubParsers);
        }

        public static Parsers.Alternative Alternative(params Parser[] SubParsers)
        {
            return new Parsers.Alternative(SubParsers);
        }

        public static Parsers.Character Character(Char C)
        {
            return new Parsers.Character(C);
        }

        public static Parsers.Expression Expression(Parser TermParser, Parser OperatorParser, OperatorTable OperatorTable)
        {
            return new Parsers.Expression(TermParser, OperatorParser, OperatorTable);
        }

        public static Parsers.Identifier Identifier(Func<Char, bool> IsLegalStartCharacter, Func<Char, bool> IsLegalCharacter)
        {
            return new Parsers.Identifier(IsLegalStartCharacter, IsLegalCharacter);
        }

        public static Parsers.KeyWord Keyword(String Word)
        {
            return new Parsers.KeyWord(Word);
        }

        public static Parsers.LateBound LateBound()
        {
            return new Parsers.LateBound();
        }

        public static Parsers.Maybe Maybe(Parser SubParser)
        {
            return new Parsers.Maybe(SubParser);
        }

        public static Parsers.NoneOrMany NoneOrMany(Parser SubParser)
        {
            return new Parsers.NoneOrMany(SubParser);
        }

        public static Parsers.OneOrMany OneOrMany(Parser SubParser)
        {
            return new Parsers.OneOrMany(SubParser);
        }

        public static Parsers.Operator Operator(OperatorTable OperatorTable)
        {
            return new Parsers.Operator(OperatorTable);
        }

        public static Parsers.Token Token(Func<Char, bool> IsLegalCharacter)
        {
            return new Parsers.Token(IsLegalCharacter);
        }

        public static Parsers.Debug Debug(Action<StringIterator> CallOnParse)
        {
            return new Parsers.Debug(CallOnParse);
        }

        public static Parser DelimitedList(Parser TermParser, Parser SeperatorParser)
        {
            return Sequence(
                    TermParser,
                    NoneOrMany(
                        Sequence(
                            SeperatorParser,
                            TermParser.PassThrough()
                        )
                    ).Flatten()
                );
        }

        #endregion

    }
}
