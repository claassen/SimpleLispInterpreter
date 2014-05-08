using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLisptInterpreter
{
    public static class Parser
    {
        public static List<Token> Parse(string source)
        {
            List<Token> tokens = new List<Token>();

            source = source.Trim();

            while (source.Length > 0)
            {
                string token = GetNextTokenString(ref source);

                if (token.StartsWith("("))
                {
                    tokens.Add(new SubExpression() { Tokens = Parse(token.Substring(1, token.Length - 2)) });
                }
                else if (IsMathOperator(token))
                {
                    tokens.Add(new MathOperator() { Type = GetMathOperatorType(token) });
                }
                else if (IsLogicalOperator(token))
                {
                    tokens.Add(new LogicalOperator() { Type = GetLogicalOperatorType(token) });
                }
                else if (IsBooleanOperator(token))
                {
                    tokens.Add(new BooleanOperator() { Type = GetBooleanOperatorType(token) });
                }
                else if (IsLambdaExpression(token))
                {
                    //(lambda (params...)(<expression>) args...)
                    string paramsExpr = GetNextTokenString(ref source);

                    var parameters = paramsExpr.Replace("(", "")
                                               .Replace(")", "")
                                               .Split(new char[] { ',' })
                                               .Select((v) => v.Trim())
                                               .ToList();
                    
                    tokens.Add(new LambdaExpression(parameters));
                }
                else
                {
                    double num;

                    if (Double.TryParse(token, out num))
                    {
                        tokens.Add(new Number(num));
                    }
                    else
                    {
                        tokens.Add(new Variable(token));
                    }
                }
            }

            return tokens;
        }

        private static string GetNextTokenString(ref string source)
        {
            string token;

            if (source[0] == '(')
            {
                int closingParentPos = MatchingParenPosition(source);

                token = source.Substring(0, closingParentPos + 1);

                source = source.Substring(closingParentPos + 1).Trim();
            }
            else if (IsMathOperator(source) || 
                     IsLogicalOperator(source) ||
                     IsBooleanOperator(source))
            {
                token = source[0].ToString();
                source = source.Substring(1).Trim();
            }
            else if (IsLambdaExpression(source))
            {
                token = "lambda";
                source = source.Substring(token.Length).Trim();
            }
            else
            {
                string[] items = source.Split(new char[] { ' ' });

                token = items[0];

                source = source.Substring(token.Length).Trim();
            }

            return token;
        }

        private static bool IsMathOperator(string c)
        {
            return c[0] == '+' || c[0] == '-' || c[0] == '*' || c[0] == '/';
        }

        private static MathOperator.OperatorType GetMathOperatorType(string c)
        {
            if (c[0] == '+')
            {
                return MathOperator.OperatorType.Plus;
            }
            else if (c[0] == '-')
            {
                return MathOperator.OperatorType.Sub;
            }
            else if (c[0] == '*')
            {
                return MathOperator.OperatorType.Mult;
            }
            else if (c[0] == '/')
            {
                return MathOperator.OperatorType.Div;
            }

            throw new Exception("Invalid math operator type");
        }

        private static bool IsLogicalOperator(string c)
        {
            return c[0] == '=' || c[0] == '<' || c[0] == '>';
        }

        private static LogicalOperator.OperatorType GetLogicalOperatorType(string c)
        {
            if (c[0] == '=')
            {
                return LogicalOperator.OperatorType.Eq;
            }
            else if (c[0] == '<')
            {
                return LogicalOperator.OperatorType.Lt;
            }
            else if (c[0] == '>')
            {
                return LogicalOperator.OperatorType.Gt;
            }

            throw new Exception("Invalid logical operator type");
        }

        private static bool IsBooleanOperator(string c)
        {
            return c[0] == '&' || c[0] == '|';
        }

        private static BooleanOperator.OperatorType GetBooleanOperatorType(string c)
        {
            if (c[0] == '&')
            {
                return BooleanOperator.OperatorType.And;
            }
            else if (c[0] == '|')
            {
                return BooleanOperator.OperatorType.Or;
            }

            throw new Exception("Invalid boolean operator type");
        }

        private static bool IsLambdaExpression(string c)
        {
            return c.StartsWith("lambda");
        }

        private static int MatchingParenPosition(string source)
        {
            int numOpen = 1;

            for (int i = 1; i < source.Length; i++)
            {
                if (source[i] == '(')
                {
                    numOpen++;
                }
                else if (source[i] == ')')
                {
                    numOpen--;
                }

                if (numOpen == 0)
                {
                    return i;
                }
            }

            throw new Exception("Mismatched parentheses");
        }
    }
}
