using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleLisptInterpreter
{
    public class RecursiveDescentParser
    {
        private static readonly string NUMBER_REGEX = @"[1-9]+[0-9]*";
        private static readonly string VARIABLE_REGEX = @"[a-zA-Z_]+[a-zA-Z_0-9]*";

        private const string LPAREN = "(";
        private const string RPAREN = ")";
        private const string PLUS = "+";
        private const string MINUS = "-";
        private const string MULT = "*";
        private const string DIV = "/";
        private const string EQ = "=";
        private const string GT = ">";
        private const string LT = "<";
        private const string GE = ">=";
        private const string LE = "<=";
        private const string AND = "&";
        private const string OR = "|";

        private const string LAMBDA = "lambda";

        private readonly List<string> RESERVED_KEYWORDS = new List<string>()
        {
            LAMBDA
        };

        private const string COMMA = ",";
        
        //Reference only
        string MATH_EXPRESSION = "(PLUS|MINUS|MULT|DIV) EXPRESSION EXPRESSION";
        string BOOLEAN_EXPRESSION = "(GT|LT|GE|LE|AND|OR) EXPRESSION EXPRESSION EXPRESSION EXPRESSION";
        string PARAMS = "'(' VARIABLE [(',' VARIABLE)*] ')'";
        string ARGS = "(EXPRESSION)+";
        string LAMBDA_EXPRESSION = "'(' LAMBDA PARAMS EXPRESSION ARGS ')'";
        string EXPRESSION = "(MATH_EXPRESSION|BOOLEAN_EXPRESSION)";
        string PROGRAM = "EXPRESSION";

        private string INPUT;

        private string CURRENT_TOKEN;

        private List<Token> Tokens = new List<Token>();

        public RecursiveDescentParser(string source)
        {
            INPUT = source;
        }

        public List<Token> Parse()
        {
            //Load first token
            Scan();

            //The whole program should be an "expression"
            Expression(Tokens);

            return Tokens;
        }

        private void Scan()
        {
            string[] tokens = Regex.Split(INPUT, @"(?=[ ,\(\)])|(?<=[ ,\(\)])");

            CURRENT_TOKEN = tokens.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));

            if (!string.IsNullOrEmpty(CURRENT_TOKEN))
            {
                INPUT = INPUT.Substring(CURRENT_TOKEN.Length).TrimStart();
            }
        }

        private void LambdaExpression(List<Token> tokens)
        {
            List<Token> paramTokens = new List<Token>();

            LambdaParams(paramTokens);

            tokens.Add(new LambdaExpression(paramTokens.Select(t => ((Variable)t).Name).ToList()));

            int numLambdaArgs = paramTokens.Count;

            Expression(tokens);

            for (int i = 0; i < numLambdaArgs; i++)
            {
                Expression(tokens);
            }
        }

        private void LambdaParams(List<Token> paramTokens)
        {
            Require(LPAREN);

            if (!Variable(paramTokens))
            {
                Error("Paramater to lambda expression");
            }

            while (true)
            {
                if (Optional(COMMA))
                {
                    if (!Variable(paramTokens))
                    {
                        Error("Paramater to lambda expression");
                    }
                }
                else
                {
                    break;
                }
            }

            Require(RPAREN);
        }

        private void Expression(List<Token> tokens)
        {
            List<Token> exprTokens = new List<Token>();

            bool hasLParen = Optional(LPAREN);

            if (Optional(PLUS, exprTokens) ||
                Optional(MINUS, exprTokens) ||
                Optional(MULT, exprTokens) ||
                Optional(DIV, exprTokens) ||
                Optional(AND, exprTokens) ||
                Optional(OR, exprTokens))
            {
                Expression(exprTokens);
                Expression(exprTokens);
            }
            else if(Optional(EQ, exprTokens) ||
                    Optional(GT, exprTokens) ||
                    Optional(LT, exprTokens) ||
                    Optional(GE, exprTokens) ||
                    Optional(LE, exprTokens))
            {
                Expression(exprTokens);
                Expression(exprTokens);
                Expression(exprTokens);
                Expression(exprTokens);
            }
            else if (Number(exprTokens) || Variable(exprTokens))
            {
                //Do nothing
            }
            else if (Optional(LAMBDA))
            {
                LambdaExpression(exprTokens);
            }
            else
            {
                Error("Expression");
            }

            if (hasLParen)
            {
                Require(RPAREN);
            }

            tokens.Add(new SubExpression(exprTokens));
        }

        private void Require(string token, List<Token> tokens = null)
        {
            if (CURRENT_TOKEN == token)
            {
                if (tokens != null)
                {
                    AddToken(CURRENT_TOKEN, tokens);
                }
                Scan();
            }
            else
            {
                Error(token);
            }
        }

        private bool Optional(string token, List<Token> tokens = null)
        {
            if (CURRENT_TOKEN == token)
            {
                if (tokens != null)
                {
                    AddToken(CURRENT_TOKEN, tokens);
                }
                Scan();
                return true;
            }
            return false;
        }

        private bool Number(List<Token> tokens)
        {
            if(Regex.IsMatch(CURRENT_TOKEN, NUMBER_REGEX))
            {
                tokens.Add(new Number(Convert.ToDouble(CURRENT_TOKEN)));
                Scan();
                return true;
            }
            return false;
        }

        private bool Variable(List<Token> tokens)
        {
            if(Regex.IsMatch(CURRENT_TOKEN, VARIABLE_REGEX) && !RESERVED_KEYWORDS.Contains(CURRENT_TOKEN))
            {
                tokens.Add(new Variable(CURRENT_TOKEN));
                Scan();
                return true;
            }
            return false;
        }

        private void Error(string expected)
        {
            throw new Exception("Unexpected token: " + CURRENT_TOKEN + ". Expecting: " + expected);
        }

        private void AddToken(string token, List<Token> tokens)
        {
            switch (token)
            {
                case LPAREN:
                case RPAREN:
                case LAMBDA:
                    //Don't add any token
                    break;
                case PLUS:
                    tokens.Add(new MathOperator(MathOperator.OperatorType.Plus));
                    break;
                case MINUS:
                    tokens.Add(new MathOperator(MathOperator.OperatorType.Sub));
                    break;
                case MULT:
                    tokens.Add(new MathOperator(MathOperator.OperatorType.Mult));
                    break;
                case DIV:
                    tokens.Add(new MathOperator(MathOperator.OperatorType.Div));
                    break;
                case EQ:
                    tokens.Add(new LogicalOperator(LogicalOperator.OperatorType.Eq));
                    break;
                case GT:
                    tokens.Add(new LogicalOperator(LogicalOperator.OperatorType.Gt));
                    break;
                case LT:
                    tokens.Add(new LogicalOperator(LogicalOperator.OperatorType.Lt));
                    break;
                case GE:
                    tokens.Add(new LogicalOperator(LogicalOperator.OperatorType.Ge));
                    break;
                case LE:
                    tokens.Add(new LogicalOperator(LogicalOperator.OperatorType.Le));
                    break;
                case AND:
                    tokens.Add(new BooleanOperator(BooleanOperator.OperatorType.And));
                    break;
                case OR:
                    tokens.Add(new BooleanOperator(BooleanOperator.OperatorType.Or));
                    break;
            }
        }
    }
}
