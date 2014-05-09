using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLisptInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            string code;

            if (args.Length > 0)
            {
                code = File.ReadAllText(args[0]).Replace("\n", "").Replace("\r", "");

                RunProgram(code);
            }
            else
            {
                do
                {
                    Console.WriteLine("Real time SimpleLisp evaluator. \nType in a line of SimpleLispCode to compile it");
                    code = Console.ReadLine();

                    RunProgram(code);
                } 
                while (!string.IsNullOrEmpty(code));
            }
        }

        private static void RunProgram(string code)
        {
            try
            {
                RecursiveDescentParser parser = new RecursiveDescentParser(code);

                List<Token> tokens = parser.Parse();

                Console.WriteLine(Interpreter.Evaluate(tokens));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
