using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Ancora
{
    public class TestGrammar : Grammar
    {
        public TestGrammar()
        {
            DefaultParserFlags = ParserFlags.IGNORE_LEADING_WHITESPACE | ParserFlags.IGNORE_TRAILING_WHITESPACE;


            var delimeters = "&%^|<>=,/-+*[]{}() \t\r\n.:;";
            var digits = "0123456789";
            var id = Identifier(c => !digits.Contains(c) && !delimeters.Contains(c), c => !delimeters.Contains(c)).CreateAst("IDENT");
            var number = Token(c => digits.Contains(c)).CreateAst("NUMBER");


            var opTable = new OperatorTable();
            opTable.AddOperator("&&", 1);
            opTable.AddOperator("||", 1);
            opTable.AddOperator("==", 1);
            opTable.AddOperator("!=", 1);
            opTable.AddOperator(">", 1);
            opTable.AddOperator("<", 1);
            opTable.AddOperator("<=", 1);
            opTable.AddOperator(">=", 1);

            opTable.AddOperator("+", 2);
            opTable.AddOperator("-", 2);

            opTable.AddOperator("*", 3);
            opTable.AddOperator("/", 3);
            opTable.AddOperator("%", 3);

            opTable.AddOperator("&", 4);
            opTable.AddOperator("|", 4);
            opTable.AddOperator("^", 4);

            var funcCall = LateBound().CreateAst("FUNCCALL") as Ancora.Parsers.LateBound;
            var op = Operator(opTable).CreateAst("OP");
            var term = (funcCall | id | number).PassThrough();
            var expression = Expression(term, op, opTable);
            var argList = Sequence(
                Character('('),
                Maybe(DelimitedList(expression, Character(','))).PassThrough(),
                Character(')')).Flatten();
            funcCall.SetSubParser((id + argList).CreateAst("FUNCCALL"));
            var typeName = LateBound();

            var typeIdentifier = (
                id.Value() +
                Maybe(
                    (Character('<') 
                    + DelimitedList(typeName, Character(',')).Flatten()
                    + '>')
                    .CreateAst("GENERICARGS")
                ))                    
                .CreateAst("TYPENAMETOKEN");

            typeName.SetSubParser(DelimitedList(typeIdentifier, Character('.')).CreateAst("TYPENAME"));

            var typeSpecifier = Character(':') + typeName;

            this.Root = typeName;
        }
    }
}
