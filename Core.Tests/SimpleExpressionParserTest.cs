using System;
using System.Collections.Generic;
using System.Linq;
using FParsec.CSharp;
using Xunit;

namespace Core.Tests
{
    public class SimpleExpressionParserTest
    {
        private readonly SimpleExpressionParser _parser;

        public static IEnumerable<object[]> TestCases { get; } = new Dictionary<string, double>
        {
            {"1", 1},
            {"1 ", 1},
            {"(1)", 1},

            {"pi", Math.PI},
            {"e", Math.E},
            
            {"2+1", 2 + 1},
            {"(((2+(1))))", 2 + 1},
            {"3+2", 3 + 2},
            
            {"3+2+4", 3 + 2 + 4},
            {"(3+2)+4", 3 + 2 + 4},
            {"3+(2+4)", 3 + 2 + 4},
            {"(3+2+4)", 3 + 2 + 4},
            
            {"3*2*4", 3 * 2 * 4},
            {"(3*2)*4", 3 * 2 * 4},
            {"3*(2*4)", 3 * 2 * 4},
            {"(3*2*4)", 3 * 2 * 4},
            
            {"3-2-4", 3 - 2 - 4},
            {"(3-2)-4", (3 - 2) - 4},
            {"3-(2-4)", 3 - (2 - 4)},
            {"(3-2-4)", 3 - 2 - 4},
            
            {"3/2/4", 3.0 / 2.0 / 4.0},
            {"(3/2)/4", (3.0 / 2.0) / 4.0},
            {"3/(2/4)", 3.0 / (2.0 / 4.0)},
            {"(3/2/4)", 3.0 / 2.0 / 4.0},
            
            {"(3*2/4)", 3.0 * 2.0 / 4.0},
            {"(3/2*4)", 3.0 / 2.0 * 4.0},
            {"3*(2/4)", 3.0 * (2.0 / 4.0)},
        }.Select(x => new object[] {x.Key, x.Value});

        public SimpleExpressionParserTest()
        {
            _parser = new SimpleExpressionParser();
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public void Test(string str, double expected)
        {
            Assert.Equal(expected, _parser.Parser.ParseString(str).Result);
        }
    }
}