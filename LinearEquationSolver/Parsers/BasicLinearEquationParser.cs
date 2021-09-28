using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LinearEquationSolver.Parsers
{
    /*
     * This parser is really bad and lazy, but it will do basic parsing.
     * TODO: Implement proper tokenizer if necessary.
     */
    public class BasicLinearEquationParser : ILinearEquationParser
    {
        private static readonly Regex sWhitespace = new Regex(@"\s+");
        private static string ReplaceWhitespace(string input, string replacement)
        {
            string ret;
            lock(sWhitespace)
            {
                ret = sWhitespace.Replace(input, replacement);
            }
            return ret;
        }

        private static List<string> SplitEquationByTerms(string eq)
        {
            if (string.IsNullOrWhiteSpace(eq)) return new List<string>();

            (string lhs, string rhs) = SplitEquationSides(eq);
            List<string> terms = new List<string>();
            StringBuilder buff = new StringBuilder();
            SplitSide(terms, buff, lhs);
            SplitSide(terms, buff, rhs, true);
            return terms;
        }

        private static (string lhs, string rhs) SplitEquationSides(string eq)
        {
            string[] split = eq.Split('=');
            string lhs, rhs;
            if (split.Length == 1)
            {
                lhs = split[0];
                rhs = "0";
            }
            else if (split.Length == 2)
            {
                lhs = split[0];
                rhs = split[1];
            }
            else
            {
                throw new FormatException($"The equation {eq} is in an invalid format.");
            }
            return (lhs, rhs);
        }

        private static void SplitSide(List<string> terms, StringBuilder buff, string side, bool reverseSign = false)
        {
            if (side == "0") return;

            bool isPositive = true;
            char sign;
            for (int i = 0; i < side.Length; i++)
            {
                char symbol = side[i];
                if (symbol == '+' || symbol == '-')
                {
                    if (i - 1 > 0 && side[i - 1] == '/')
                    {
                        /*
                         * In the special case where the sign is in the denominator of a fraction only change the sign of the term.
                         * Even though this case might be considered illegal it's still handled here.
                        */
                        isPositive = symbol == '+' ? true : false;
                        continue;
                    }

                    if (buff.Length != 0)
                    {
                        if (reverseSign)
                            sign = isPositive ? '-' : '+';
                        else
                            sign = isPositive ? '+' : '-';

                        terms.Add(sign + buff.ToString());
                        buff.Clear();
                    }

                    isPositive = symbol == '+' ? true : false;
                }
                else
                {
                    buff.Append(symbol);
                }
            }

            if (reverseSign)
                sign = isPositive ? '-' : '+';
            else
                sign = isPositive ? '+' : '-';

            terms.Add(sign + buff.ToString());
            buff.Clear();
        }

        private static (string leading, string rest) CutLeading(string rawTerm, Predicate<char> pred)
        {
            if (rawTerm == "") return ("", "");

            int i;
            for (i = 0; i < rawTerm.Length; i++)
            {
                if (!pred(rawTerm[i]))
                    break;
            }

            string leading = rawTerm.Substring(0, i);
            string rest = rawTerm.Substring(i, rawTerm.Length - i);
            return (leading, rest);
        }

        private static (string coefficient, string variable) SplitVarAndCoef(string input)
        {
            if (input == "") return ("", "");

            string coefficient, variable;
            (coefficient, input) = CutLeading(input, x => char.IsDigit(x));
            if (coefficient.Length != 0)
            {
                (variable, input) = CutLeading(input, x => char.IsLetter(x));
                if (input.Length != 0) // sanity check
                    throw new FormatException("Term format is invalid");
            }
            else
            {
                (variable, input) = CutLeading(input, x => char.IsLetter(x));
                (coefficient, input) = CutLeading(input, x => char.IsDigit(x));
                if (input.Length != 0) // sanity check
                    throw new FormatException("Term format is invalid");
            }

            return (coefficient, variable);
        }

        public LinearEquation Parse(string rawInput)
        {
            rawInput = ReplaceWhitespace(rawInput, "");
            List<string> terms = SplitEquationByTerms(rawInput);
            LinearEquation eq = new LinearEquation();

            for (int i = 0; i < terms.Count; i++)
            {
                string rawTerm = terms[i];
                bool isPositive = rawTerm[0] == '+' ? true : false;
                rawTerm = rawTerm.Substring(1, rawTerm.Length - 1);
                if (rawTerm.IndexOf('/') != -1)
                {
                    // Is a fraction
                    string[] fractionSplit = rawTerm.Split('/');
                    if (fractionSplit.Length != 2)  // sanity check
                        throw new FormatException("Term format is invalid");

                    string variable;
                    string numberatorStr, numberatorVar;
                    string denominatorStr, denominatorVar;

                    (numberatorStr, numberatorVar) = SplitVarAndCoef(fractionSplit[0]);
                    (denominatorStr, denominatorVar) = SplitVarAndCoef(fractionSplit[1]);

                    long numberator = 1;
                    long denominator = 1;
                    if (numberatorStr != "")
                        _ = long.TryParse(numberatorStr, out numberator);
                    if (denominatorStr != "")
                        _ = long.TryParse(denominatorStr, out denominator);

                    if (numberatorVar != "" && denominatorVar != "")
                    {
                        throw new FormatException("Term format is invalid");
                    }
                    else if (denominatorVar != "" && denominatorStr == "" )
                    {
                        // equation is non-linear in this case !
                        throw new FormatException("Term format is invalid");
                    }
                    else if (denominatorVar != "")
                    {
                        variable = denominatorVar;
                    }
                    else if (numberatorVar != "")
                    {
                        variable = numberatorVar;
                    }
                    else
                    {
                        variable = "";
                    }

                    numberator = isPositive ? numberator : -numberator;
                    Fraction f = new Fraction(numberator, denominator);
                    Term t = new Term(f, variable);
                    eq.AddTerm(t);
                }
                else
                {
                    (string numberatorStr, string variable) = SplitVarAndCoef(rawTerm);
                    long numberator = 1;
                    if (numberatorStr != "")
                        _ = long.TryParse(numberatorStr, out numberator);

                    numberator = isPositive ? numberator : -numberator;
                    Fraction f = new Fraction(numberator, 1);
                    Term t = new Term(f, variable);
                    eq.AddTerm(t);
                }
            }

            return eq;
        }
    }
}
