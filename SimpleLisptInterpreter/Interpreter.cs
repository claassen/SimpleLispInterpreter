using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLisptInterpreter
{
    public static class Interpreter
    {
        public static Value Evaluate(Token token, Dictionary<string, Value> scopeVariables = null)
        {
            return Evaluate(new List<Token>() { token }, scopeVariables);
        }

        public static Value Evaluate(List<Token> tokens, Dictionary<string, Value> scopeVariables = null)
        {
            if (tokens.Count == 0)
            {
                throw new Exception("Invalid token sequence");
            }

            Queue<Token> q = new Queue<Token>(tokens);

            Value val = null;

            Token t = q.Dequeue();

            try
            {
                if (t is SubExpression)
                {
                    val = Evaluate(((SubExpression)t).Tokens, scopeVariables);
                }
                else if (t is Number)
                {
                    val = new NumberValue(((Number)t).Value);
                }
                else if (t is Variable)
                {
                    Variable v = ((Variable)t);

                    if (scopeVariables != null && scopeVariables.ContainsKey(v.Name))
                    {
                        val = scopeVariables[v.Name];
                    }
                    else
                    {
                        throw new Exception("Unknown variable: " + v.Name);
                    }
                }
                else if (t is MathOperator)
                {
                    MathOperator op = (MathOperator)t;

                    Value op1 = Evaluate(q.Dequeue(), scopeVariables);
                    Value op2 = Evaluate(q.Dequeue(), scopeVariables);

                    if (!(op1 is NumberValue) || !(op2 is NumberValue))
                    {
                        throw new Exception("Invalid operand types for operator: " + op.ToString());
                    }

                    val = op.Evaluate((NumberValue)op1, (NumberValue)op2);
                }
                else if (t is LogicalOperator)
                {
                    LogicalOperator op = (LogicalOperator)t;

                    Value op1 = Evaluate(q.Dequeue(), scopeVariables);
                    Value op2 = Evaluate(q.Dequeue(), scopeVariables);

                    if (!(op1 is NumberValue) || !(op2 is NumberValue))
                    {
                        throw new Exception("Invalid operand types for operator: " + op.ToString());
                    }

                    Token t1 = q.Dequeue();
                    Token t2 = q.Dequeue();

                    val = op.Evaluate((NumberValue)op1, (NumberValue)op2).Value ? Evaluate(t1, scopeVariables) 
                                                                                : Evaluate(t2, scopeVariables);
                }
                else if (t is BooleanOperator)
                {
                    BooleanOperator op = (BooleanOperator)t;

                    Value op1 = Evaluate(q.Dequeue(), scopeVariables);
                    Value op2 = Evaluate(q.Dequeue(), scopeVariables);

                    if (!(op1 is BooleanValue) || !(op2 is BooleanValue))
                    {
                        throw new Exception("Invalid operand types for operator: " + op.ToString());
                    }

                    Token t1 = q.Dequeue();
                    Token t2 = q.Dequeue();

                    val = op.Evaluate((BooleanValue)op1, (BooleanValue)op2).Value ? Evaluate(t1, scopeVariables) 
                                                                                  : Evaluate(t2, scopeVariables);
                }
                else if (t is LambdaExpression)
                {
                    LambdaExpression lambda = (LambdaExpression)t;

                    Token expr = q.Dequeue();

                    if (!(expr is SubExpression))
                    {
                        throw new Exception("Unexpected token after lambda expression. Expecting sub expression");
                    }

                    var paramValues = new Dictionary<string, Value>();

                    foreach (string param in lambda.Parameters)
                    {
                        paramValues.Add(param, Evaluate(q.Dequeue()));
                    }

                    val = Evaluate(expr, paramValues);
                }
                else
                {
                    throw new Exception("Unexpected token in sequence: " + t.ToString());
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception("Expected token missing");
            }

            if (q.Count > 0)
            {
                throw new Exception("Too many tokens in sequence");
            }

            return val;
        }
    }
}
