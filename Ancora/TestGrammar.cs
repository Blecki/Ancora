﻿using System;
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
            var delimeters = "&%^|<>=,/-+*[]{}() \t\r\n.:;";
            var digits = "0123456789";
            var whitespace = Token(c => " \t\r\n".Contains(c));
            var identifier = Identifier(c => !digits.Contains(c) && !delimeters.Contains(c), c => !delimeters.Contains(c)).CreateAst("IDENT");
            var optionalWhitespace = Maybe(whitespace);
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
            var term = optionalWhitespace + (funcCall | identifier | number).PassThrough() + optionalWhitespace;
            var expression = Expression(term, op, opTable);
            var argList = Sequence(
                optionalWhitespace,
                Character('('),
                optionalWhitespace,
                Maybe(expression * (optionalWhitespace + ',' + optionalWhitespace)).PassThrough(),
                optionalWhitespace,
                Character(')')).Flatten();
            funcCall.SetSubParser(Sequence(identifier, optionalWhitespace, argList).CreateAst("FUNCCALL"));
            var typeName = LateBound();

            var typeIdentifier = (
                optionalWhitespace +
                identifier.Value() +
                Maybe(
                    (optionalWhitespace + '<' 
                    + optionalWhitespace 
                    + (typeName * (optionalWhitespace + ',' + optionalWhitespace)).Flatten()
                    + optionalWhitespace + '>' + optionalWhitespace)
                    .CreateAst("GENERICARGS")
                ))                    
                .CreateAst("TYPENAMETOKEN");

            typeName.SetSubParser(DelimitedList(typeIdentifier, Sequence(optionalWhitespace, Character('.'), optionalWhitespace)).CreateAst("TYPENAME"));

            var typeSpecifier = Character(':') + typeName;

            this.Root = typeName;
        }
    }
}
