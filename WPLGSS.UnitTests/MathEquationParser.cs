using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace WPLGSS.Utilities.UnitTests
{
    public class MathEquationParserTest
    {
        [Fact]
        public void Add()
        {
            var parser = new MathEquationParser();
            var func = parser.ParseFunction("1+1");
            Assert.Equal(2, func(0), 3);
        }
        [Fact]
        public void Subtract()
        {
            var parser = new MathEquationParser();
            var func = parser.ParseFunction("1-1");
            Assert.Equal(0, func(0), 3);
        }
        [Fact]
        public void Multiply()
        {
            var parser = new MathEquationParser();
            var func = parser.ParseFunction("20*5");
            Assert.Equal(100, func(0), 3);
        }
        [Fact]
        public void Divide()
        {
            var parser = new MathEquationParser();
            var func = parser.ParseFunction("20/5");
            Assert.Equal(4, func(0), 3);
        }
        [Fact]
        public void Power()
        {
            var parser = new MathEquationParser();
            var func = parser.ParseFunction("10^2");
            Assert.Equal(100, func(0), 3);
        }
        [Fact]
        public void Variables()
        {
            var parser = new MathEquationParser();
            var func = parser.ParseFunction("x + 5");
            Assert.Equal(6, func(1));
        }
        [Fact]
        public void SubExpressions()
        {
            var parser = new MathEquationParser();
            var func = parser.ParseFunction("(10^2 - 10)/9");
            Assert.Equal(10, func(0), 3);
            var func2Groups = parser.ParseFunction("(2+5) - (3+6)");
            Assert.Equal(func2Groups(0), -2, 3);
        }
        [Fact]
        public void Functions()
        {
            var parser = new MathEquationParser();
            var func = parser.ParseFunction("sin(pi)");
            Assert.Equal(0, func(0), 10);
        }
        [Fact]
        public void NegateFirstNumber()
        {
            var parser = new MathEquationParser();
            var func = parser.ParseFunction("-7");
            Assert.Equal(-7, func(0));
        }

        [Fact]
        public void Parameterization()
        {
            var parser = new MathEquationParser();
            var func = parser.ParseFunction("x^2 + 7x");
            Assert.Equal(8, func(1));
        }

        [Fact]
        public void NegateParameter()
        {
            var parser = new MathEquationParser();
            var func = parser.ParseFunction("-x");
            Assert.Equal(-1, func(1));
        }

        [Fact]
        public void AddANegative()
        {
            var parser = new MathEquationParser();
            var func = parser.ParseFunction("3 + -1");
            Assert.Equal(2, func(0));
        }

        [Fact]
        public void FunctionWithInvalidVariable()
        {
            var parser = new MathEquationParser();
            Assert.Throws<ArgumentException>(() => parser.ParseFunction("1 + y"));
        }
    }
}
