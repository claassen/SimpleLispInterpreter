using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLisptInterpreter
{
    public class Token
    {
    }

    public class SubExpression : Token
    {
        public List<Token> Tokens;
    }

    public class Number : Token
    {
        public double Value;

        public Number(double value)
        {
            Value = value;
        }
    }

    public class Variable : Token
    {
        public string Name;
        
        public Variable(string name)
        {
            Name = name;
        }
    }

    public class MathOperator : Token
    {
        public enum OperatorType
        {
            Plus,
            Sub,
            Mult,
            Div
        }

        public OperatorType Type;

        public NumberValue Evaluate(NumberValue x, NumberValue y)
        {
            switch (Type)
            {
                case OperatorType.Plus:
                    return new NumberValue(x.Value + y.Value);
                case OperatorType.Sub:
                    return new NumberValue(x.Value - y.Value);
                case OperatorType.Mult:
                    return new NumberValue(x.Value * y.Value);
                case OperatorType.Div:
                    return new NumberValue(x.Value / y.Value);
            }

            throw new Exception("Invalid operator type");
        }
    }

    public class LogicalOperator : Token
    {
        public enum OperatorType
        {
            Eq,
            Gt,
            Lt
        }

        public OperatorType Type;

        public BooleanValue Evaluate(NumberValue x, NumberValue y)
        {
            switch (Type)
            {
                case OperatorType.Eq:
                    return new BooleanValue(x.Value == y.Value);
                case OperatorType.Lt:
                    return new BooleanValue(x.Value < y.Value);
                case OperatorType.Gt:
                    return new BooleanValue(x.Value > y.Value);
            }

            throw new Exception("Invalid operator type");
        }
    }

    public class BooleanOperator : Token
    {
        public enum OperatorType
        {
            And,
            Or
        }

        public OperatorType Type;

        public BooleanValue Evaluate(BooleanValue x, BooleanValue y)
        {
            switch (Type)
            {
                case OperatorType.And:
                    return new BooleanValue(x.Value && y.Value);
                case OperatorType.Or:
                    return new BooleanValue(x.Value || y.Value);
            }

            throw new Exception("Invalid operator type");
        }
    }

    public class LambdaExpression : Token
    {
        public List<string> Parameters;

        public LambdaExpression(List<string> parameters)
        {
            Parameters = parameters;
        }
    }
}
