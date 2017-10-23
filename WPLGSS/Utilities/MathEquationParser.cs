using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace WPLGSS.Utilities
{
    /// <summary>
    /// A utility class that can transform a string into an exectutable function.
    /// </summary>
    public sealed class MathEquationParser
    {
        private ParameterExpression x = Expression.Parameter(typeof(double),"x");

        private List<Expression> subExpressions = new List<Expression>();

        private Dictionary<string, System.Reflection.MethodInfo> functions;

        /// <summary>
        /// Parses a function as a string into an executable function.
        /// </summary>
        /// <param name="function">The string to parse.</param>
        /// <returns>The function the string represents.</returns>
        public Func<double, double> ParseFunction(string function)
        {
            return ParseFunctionAsExpressionTree(function).Compile();
        }

        /// <summary>
        /// Parses a function as a string into an expression tree.
        /// </summary>
        /// <param name="function">The string to parse.</param>
        /// <returns>The expression tree representing the function.</returns>
        public Expression<Func<double, double>> ParseFunctionAsExpressionTree(string function)
        {
            function = Regex.Replace(function, @"({|\[)", "(");//Replace all non-paren grouping symbols with parens
            function = Regex.Replace(function, @"(}|\])", ")");
            function = Regex.Replace(function, @"\s", "");
            subExpressions.Clear();
            subExpressions.Add(x);
            subExpressions.Add(Expression.Constant(Math.E));
            subExpressions.Add(Expression.Constant(Math.PI));
            function = function.Replace(nameof(x), "{0}");
            function = function.Replace("e", "{1}");
            function = function.Replace("pi", "{2}");
            function = Regex.Replace(function, @"(?<number>[0-9]+){", @"${number}*{"); // Handles expressions such as 5x and changes it to 5 * x
            if (function[0] == '-')
            {
                if (function[1] != '{') // If we are not negating a subexpression
                {
                    using (TypeTextReader reader = new TypeTextReader(new StringReader(function.Substring(1))))
                    {
                        var val = reader.ReadDouble();
                        var exprID = subExpressions.Count;
                        subExpressions.Add(Expression.Constant(-val));
                        var length = function.Length - reader.BaseReader.ReadToEnd().Length;
                        function = function.Replace(function.Substring(0, length), "{" + exprID + "}");
                    } 
                }
                else
                {
                    using (TypeTextReader reader = new TypeTextReader(new StringReader(function.Substring(1))))
                    {
                        var subExprId = (int)reader.ReadInt64();
                        var exprID = subExpressions.Count;
                        subExpressions.Add(Expression.Negate(subExpressions[subExprId]));
                        var length = function.Length - reader.BaseReader.ReadToEnd().Length;
                        function = function.Replace(function.Substring(0, length), "{" + exprID + "}");
                    }
                }
            }
            var lambda = Expression.Lambda<Func<double, double>>(ParseEquation(function), x);
            return lambda;
        }

        private Expression ParseEquation(string function)
        {
            var funcs = false;
            foreach (char item in function)
            {
                funcs |= Char.IsLetter(item);
                if (funcs)
                {
                    break;
                }
            }
            if (!funcs)
            {
                return ParseEquationNoFunctions(function);
            }
            else
            {
                return ParseEquationWithFunctions(function);
            }
        }

        private Expression ParseEquationWithFunctions(string function)
        {
            if (functions == null)
            {
                InitializeFunctions();
            }
            var funcNameStartIndex = function.IndexOf(function.First(character => Char.IsLetter(character)));
            var parameterListStartIndex = function.IndexOf('(');
            if (parameterListStartIndex == -1)
            {
                throw new ArgumentException("Could not find any parameters for a function. Are you sure you spelled the function names correctly and that your variable name is x?");
            }
            var name = function.Substring(funcNameStartIndex, parameterListStartIndex - funcNameStartIndex);
            var argStartIndex = function.IndexOf(name) + name.Length + 1;//Skip '('
            var funcAfterName = function.Substring(argStartIndex);
            var argStr = GetSubExpression(funcAfterName);
            var arg = ParseEquation(argStr);
            if (!functions.TryGetValue(name, out var method))
            {
                throw new ArgumentException($"Could not find function named {name}. Are you sure you spelled it correctly?");
            }
            subExpressions.Add(Expression.Call(method, arg));
            return ParseEquation(function.Replace(name + '(' + argStr + ')', '{' + (subExpressions.Count - 1).ToString() + '}'));
        }

        private static string GetSubExpression(string eqFromSubExprStart)
        {
            var argLength = 0;
            for (int nestLevel = 1; nestLevel > 0 && argLength <= eqFromSubExprStart.Length; ++argLength)
            {
                var charAt = eqFromSubExprStart[argLength];
                if (charAt == '(')
                {
                    ++nestLevel;
                }
                else if (charAt == ')')
                {
                    --nestLevel;
                }
            }
            var argStr = eqFromSubExprStart.Substring(0, argLength - 1);
            return argStr;
        }

        private void InitializeFunctions()
        {
            functions = new Dictionary<string, System.Reflection.MethodInfo>
            {
                { "sin", typeof(Math).GetMethod("Sin") },
                { "cos", typeof(Math).GetMethod("Cos") },
                { "tan", typeof(Math).GetMethod("Tan") },
                { "arcsin", typeof(Math).GetMethod("Asin") },
                { "arccos", typeof(Math).GetMethod("Acos") },
                { "arctan", typeof(Math).GetMethod("Atan") },
                { "ln", typeof(Math).GetMethod("Log", new Type[] { typeof(double) }) }
            };
        }

        private Expression ParseEquationNoFunctions(string equation)
        {
            while (equation.IndexOf('(') != -1)//While there are still grouping symbols
            {
                var subEquation = GetSubExpression(equation.Substring(equation.IndexOf('(') + 1));//Skip '('
                subExpressions.Add(ParseEquationNoFunctions(subEquation));
                equation = equation.Replace("(" + subEquation + ")", "{" + (subExpressions.Count - 1).ToString() + "}");
            }
            CreateExpressions(ref equation, '^');
            CreateExpressions(ref equation, '*', '/');
            CreateExpressions(ref equation, '-', '+');
            using (TypeTextReader reader = new TypeTextReader(new StringReader(equation.Substring(1))))
            {
                var expressionID = (int)reader.ReadInt64();
                return subExpressions[expressionID];
            }
        }

        private void CreateExpressions(ref string equation, params char[] operations)
        {
            var index = 0;
            while ((index = equation.IndexOfAny(operations)) != -1)// While one of the operations can still be performed
            {

                var previous = equation[index - 1];
                Expression left, right;
                int startPlaceholder, endPlaceholder;
                if (previous == '}')
                {
                    var openingIndex = equation.Substring(0, index).LastIndexOf('{');
                    var exprID = Int32.Parse(equation.Substring(openingIndex + 1, (index - 1) - (openingIndex + 1)));
                    left = subExpressions[exprID];
                    startPlaceholder = openingIndex;
                }
                else
                {
                    var substr = new string(equation.Substring(0, index).Reverse().ToArray());
                    var leftVal = Double.NaN;
                    var leftBuilder = new StringBuilder();
                    using (StringReader reader = new StringReader(substr))
                    {
                        var val = 0;
                        while ((val = reader.Read()) != -1)
                        {
                            var ch = (char)val;
                            if (Char.IsDigit(ch))
                            {
                                leftBuilder.Append(ch);
                            }
                            else
                            {
                                break;
                            }
                        }
                        leftVal = Double.Parse(new string(leftBuilder.ToString().Reverse().ToArray()));
                    }
                    left = Expression.Constant(leftVal);
                    startPlaceholder = equation.Substring(0, index).LastIndexOf(leftVal.ToString());
                }
                var next = equation[index + 1];
                if (next == '{')
                {
                    var closingIndex = equation.Substring(index).IndexOf('}');
                    int exprID;
                    using (TypeTextReader reader = new TypeTextReader(new StringReader(equation.Substring(index + 2))))
                    {
                        exprID = (int)reader.ReadInt64();
                    }
                    right = subExpressions[exprID];
                    endPlaceholder = closingIndex + index + 1;
                }
                else
                {
                    using (TypeTextReader reader = new TypeTextReader(new StringReader(equation.Substring(index + 1))))
                    {
                        var exp = reader.ReadDouble();
                        right = Expression.Constant(exp);
                        endPlaceholder = equation.Length - reader.BaseReader.ReadToEnd().Length;
                    }
                }
                var op = equation[index];
                equation = equation.Replace(equation.Substring(startPlaceholder, endPlaceholder - startPlaceholder), "{" + subExpressions.Count.ToString() + "}");
                subExpressions.Add(Expression.MakeBinary(ExpTypeFromChar(op), left, right));
            }
        }

        private ExpressionType ExpTypeFromChar(char operation)
        {
            switch (operation)
            {
                case '+':
                    return ExpressionType.Add;
                case '-':
                    return ExpressionType.Subtract;
                case '*':
                    return ExpressionType.Multiply;
                case '/':
                    return ExpressionType.Divide;
                case '^':
                    return ExpressionType.Power;
                default:
                    throw new ArgumentException("Operation character not supported.");
            }
        }
    }
}
