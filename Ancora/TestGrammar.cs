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
            
            var delimeters = "&%^|<>=,/-+*[]{}() \t\r\n.:;\"@";
            var digits = "0123456789";
            var id = Identifier(c => !digits.Contains(c) && !delimeters.Contains(c), c => !delimeters.Contains(c)).CreateAst("IDENT");
            var numberLiteral = Token(c => digits.Contains(c)).CreateAst("NUMBERLIT");

            #region Setup Operator Table

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

            #endregion

            var expression = LateBound();
            var statement = LateBound();
            var funcCall = LateBound().CreateAst("FUNCCALL") as Ancora.Parsers.LateBound;
            var term = LateBound();
            var macroInvokation = LateBound();

            #region Type Names
            var typeName = LateBound();
            var typeIdentifier = (id.Value() + Maybe(('<' + DelimitedList(typeName, ',').Flatten() + '>').CreateAst("GENERICARGS"))).CreateAst("TYPENAMETOKEN");
            typeName.SetSubParser(DelimitedList(typeIdentifier, '.').CreateAst("TYPENAME"));
            var typeSpecifier = ':' + typeName;
            #endregion

            #region Strings
            var embeddedExpression = ('[' + expression + ']').CreateAst("EMBEDDED");
            var text = Token(c => c != '\\' && c != '\"' && c != '[').CreateAst("TEXT");
            var escapeSequence = ('\\' + (Character('n') | '\\' | '\"')).CreateAst("ESCAPE");
            var _string = ('\"' + NoneOrMany(escapeSequence | text).Flatten() + '\"').CreateAst("STRING");
            var complexString = (Character('@') + '\"' + NoneOrMany(escapeSequence | embeddedExpression | text).Flatten() + '\"').CreateAst("COMPLEXSTRING");
            #endregion

            #region UnitTypes
            var unit = (Token(c => c >= 'a' && c <= 'z').Value() 
                + Maybe(('^' + numberLiteral.Value()).CreateAst("POWER"))).CreateAst("UNIT");
            var unitSet = OneOrMany(unit).CreateAst("UNITSET");
            var unitType = (unitSet + Maybe('/' + unitSet.PassThrough())).CreateAst("UNITTYPE");
            var number = (numberLiteral.Value() + Maybe(unitType)).CreateAst("NUMBER");
            #endregion

            #region Blocks and Bodies

            var lambdaExpression = Keyword("=>") + expression;
            var block = '{' + NoneOrMany(statement) + '}';
            var body = statement | block;
            var functionBlock = body + Maybe(Keyword("error" + body)) + Maybe(Keyword("post" + body));

            #endregion

            #region Declarations

            var ruleModifier = id;
            var whenClause = Keyword("when") + functionBlock;
            var ruleName = OneOrMany(id);
            var ruleDeclaration = Maybe(ruleModifier) + ruleName + '(' + Maybe(DelimitedList(id + typeSpecifier, ',')) + ')' + typeSpecifier +
                Maybe(whenClause) + functionBlock;
            var rule = Keyword("rule") + ruleDeclaration;

            var functionDeclaration = id + '(' + Maybe(DelimitedList(id + typeSpecifier, ',')) + ')' + typeSpecifier + functionBlock;
            var methodDeclaration = Keyword("method") + functionDeclaration;
            var fieldDeclaration = Keyword("var") + id + typeSpecifier + Maybe(expression) + ';';
            var memberDeclaration = fieldDeclaration | methodDeclaration | rule;
            var typeDeclaration = Keyword("type") + typeIdentifier + Maybe(typeSpecifier) + '{' + NoneOrMany(memberDeclaration) + '}';
            var declarationTerm = id | ( '(' + id + typeSpecifier + ')' );
            var macro = Keyword("macro") + OneOrMany(declarationTerm) + Maybe(typeSpecifier) + functionBlock;
            var function = Keyword("func") + functionDeclaration;
            var _namespace = Keyword("namespace") + typeIdentifier + '{' + NoneOrMany(typeDeclaration) + '}';

            var headerUsing = Keyword("using") + typeName + ';';
            var headerBlock = NoneOrMany(headerUsing);

            var file = headerBlock + NoneOrMany(_namespace | typeDeclaration | function | macro | rule);

            var lambda = Keyword("lambda") + functionDeclaration;

            #endregion

            #region New
            var memberInitializer = Keyword("let") + id + "=" + expression + ";";
            var memberInitializerBlock = '{' + NoneOrMany(memberInitializer) + '}';
            var _new = Keyword("new") + typeName + (memberInitializerBlock | ';');
            #endregion

            #region Function Calls

            var argList = ('(' + Maybe(DelimitedList(expression, Character(','))).PassThrough() + ')').Flatten();
            funcCall.SetSubParser((term + argList).CreateAst("FUNCCALL"));
            macroInvokation.SetSubParser(OneOrMany(term | id | argList) + (functionBlock | ';'));
            var consider = Keyword("consider") + ruleName + argList;
            var follow = Keyword("follow") + ruleName + argList;
            var _value = Keyword("value") + Maybe(Keyword("of")) + ruleName + argList;

            #endregion

            #region Terms and Operators

            var parenthetical = '(' + expression + ')';
            var op = Operator(opTable).CreateAst("OP");
            expression.SetSubParser(Expression(term, op, opTable));
            var indexer = term + '@' + term;
            var memberAccess = term + '.' + id;
            var cast = term + Keyword(">>>") + typeName;
            var bracketInvokation = '[' + macroInvokation + ']';
            var list = '{' + Maybe(DelimitedList(term, ',')) + '}';
            term.SetSubParser((consider | follow | _value | funcCall | memberAccess | indexer | cast | id | number | _new | _string | complexString | parenthetical | bracketInvokation | list | lambda).PassThrough());
            
            #endregion

            #region Statements
            var local = Keyword("var") + id + Maybe(typeSpecifier) + Maybe('=' + expression);
            var lvalue = id | memberAccess | indexer;
            var let = Keyword("let") + lvalue + '=' + expression;
            var _if = Keyword("if") + parenthetical + functionBlock + Maybe(Keyword("else") + functionBlock);
            var loop = Keyword("loop") + Maybe(id) + functionBlock;
            var _break = Keyword("break") + Maybe(id);
            var _continue = Keyword("continue") + Maybe(id);
            var _return = Keyword("return") + Maybe(expression);
            statement.SetSubParser( ((local | let | _if | loop | _break | _continue | _return | consider | follow | funcCall) + ';') | macroInvokation);
            #endregion

            this.Root = file;
        }
    }
}
