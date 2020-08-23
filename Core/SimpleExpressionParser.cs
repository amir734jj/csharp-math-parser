using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FParsec;
using FParsec.CSharp;
using Microsoft.FSharp.Core;
using static FParsec.CSharp.CharParsersCS;
using static FParsec.CSharp.PrimitivesCS;

namespace Core
{
    public class SimpleExpressionParser
    {
        public FSharpFunc<CharStream<Unit>, Reply<double>> Parser;

        private static int Factorial(int n) => n == 0 ? 1 : n * Factorial(n - 1);

        private static FSharpFunc<CharStream<Unit>, Reply<double>> MathFunc(FSharpFunc<CharStream<Unit>,Reply<double>> term, string key, Func<double, double> func)
        {
            var noParenthesis = Pipe(StringP(key).And(WS), term, (_, d) => func(d));
            var withParenthesis = Pipe(StringP(key).And(WS), CharP('(').And(WS), term.And(WS), CharP(')'), (_, pl, d, pr) => func(d));

            return noParenthesis.Or(withParenthesis);
        }
        
        public SimpleExpressionParser()
        {
            var piParser = StringP("pi").Return(Math.PI);
            var eParser = StringP("e").Return(Math.E);
            
            var thing = Float.Or(piParser).Or(eParser);

            Parser =
                WS.And(new OPPBuilder<Unit, double, Unit>()
                    .WithOperators(ops => ops
                        .AddInfix("+", 10, WS, (x, y) => x + y)
                        .AddInfix("-", 10, WS, (x, y) => x - y)
                        .AddInfix("*", 20, WS, (x, y) => x * y)
                        .AddInfix("/", 20, WS, (x, y) => x / y)
                        .AddInfix("%", 20, WS, (x, y) => x % y)
                        .AddPrefix("-", 20, x => -x)
                        .AddInfix("^", 30, Associativity.Right, WS, (x, y) => (int) Math.Pow(x, y))
                        .AddPostfix("!", 40, x => Factorial(Convert.ToInt32(x))))
                    .WithTerms(term =>
                    {
                        var aTan = MathFunc(term, "atan", Math.Atan);
                        var aSin = MathFunc(term, "asin", Math.Asin);
                        var tan = MathFunc(term, "tan", Math.Tan); 
                        var sin = MathFunc(term, "sin", Math.Sin);
                        var log = MathFunc(term, "log", Math.Log);
                        var log10 = MathFunc(term, "log10", Math.Log10);
                        var ln = MathFunc(term, "ln", x => Math.Log(x, Math.E));

                        var recStuff = aTan.Or(aSin).Or(tan).Or(sin).Or(log).Or(log10).Or(ln);
                        
                        return Choice(
                            WS.And(thing.Or(recStuff)).And(WS),
                            Between(CharP('(').And(WS), term, CharP(')').And(WS)));
                    })
                    .Build()
                    .ExpressionParser);
        }
    }
}