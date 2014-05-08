using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLisptInterpreter
{
    public class Value
    {
    }

    public class NumberValue : Value
    {
        public double Value;

        public NumberValue(double val)
        {
            Value = val;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class BooleanValue : Value
    {
        public bool Value;

        public BooleanValue(bool val)
        {
            Value = val;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class StringValue : Value
    {

    }
}
