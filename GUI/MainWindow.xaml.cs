using System;
using System.Windows;
using System.Windows.Media.Imaging;
using CSharpMath.SkiaSharp;
using System.IO;
using LinearEquationSolver.Parsers;
using LinearEquationSolver;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string USER_INPUT_ERR = "Failed to convert user input into a system. \n" +
            "Please provide valid input. For Example: \n" +
            "x + y − z = 10 \n" +
            "2x − 2y + z = 0 \n" +
            "x + z = 5 \n" +
            "4y + z = 20";

        private StringBuilder latexBuf;

        public MainWindow()
        {
            this.latexBuf = new StringBuilder();
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        private void BtnSolve_Click(object sender, RoutedEventArgs e)
        {
            string userInput = textBoxEquation.Text;
            if (userInput == null) return;

            LinearSystem ls;
            try
            {
                // Parse user input:
                ls = UserInputToLinearSystem(userInput);
            }
            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show(USER_INPUT_ERR, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                Console.WriteLine(ex.Message);
                return;
            }

            lock (latexBuf) // Pointless in non multi-threaded code, but leave as a reminder.
            {
                latexBuf.Clear();
                try
                {
                    // Parse system solution to latex output:
                    SystemSolutionToLatex(ls, latexBuf);
                }
                catch (Exception ex)
                {
                    MessageBoxResult result = MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine(ex.Message);
                    return;
                }

                // Create latex bitmap and set the image control:
                BitmapImage bitmap = LatexToBitmap(latexBuf);
                imgDynamic.Source = bitmap;
                imgDynamic.Width = bitmap.Width;
                imgDynamic.Height = bitmap.Height;
            }
        }

        private static BitmapImage LatexToBitmap(StringBuilder latexBuf)
        {
            string latex = latexBuf.ToString();
            MathPainter painter = new MathPainter();
            painter.LaTeX = latex;
            Stream bitmapStream = painter.DrawAsStream();

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = bitmapStream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        private static LinearSystem UserInputToLinearSystem(string userInput)
        {
            // Make equation out of user input:
            ILinearEquationParser lep = new BasicLinearEquationParser();
            string[] split = userInput.Replace("−", "-").Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            LinearSystem ls = new LinearSystem();
            foreach (string rawEq in split)
            {
                LinearEquation eq = lep.Parse(rawEq);
                ls.AddEquation(eq);
            }

            return ls;
        }

        private static void SystemSolutionToLatex(LinearSystem ls, StringBuilder latexBuf)
        {
            // FIXME: remove this:
            ls.SetSystemWideComparison(ComparisonFactory.CreateVariableOrderComparison(new Dictionary<string, int>() {
                { "x", 5 }, { "y", 4 }, { "z", 3 }, { "w", 2 },
            }));

            LinearSystem.SubstitutionResult subResult;
            LatexDisplayMgr.LinearSystemToLatex(latexBuf, ls);
            while (true)
            {
                subResult = ls.SubstituteEquations();
                while (subResult == LinearSystem.SubstitutionResult.SUBSTITUTION_OCCURRED)
                {
                    subResult = ls.SubstituteEquations();
                }
                if (subResult == LinearSystem.SubstitutionResult.CONTRADICTION)
                {
                    break;
                }

                LinearSystem.ReductionResult res = ls.ReduceEquation();
                if (res == null)
                {
                    break;
                }

                string reducedByStr = res.ToString().Replace("*", @"\cdot");
                latexBuf.Append(reducedByStr);
                latexBuf.AppendLine(@"\longrightarrow \quad");
                LatexDisplayMgr.LinearSystemToLatex(latexBuf, ls, res.SrcReduceRow, res.DestReduceRow);
            }

            if (subResult != LinearSystem.SubstitutionResult.CONTRADICTION)
            {
                bool wroteSolution = LatexDisplayMgr.SolutionToLatex(latexBuf, ls);
                if (!wroteSolution)
                {
                    latexBuf.Append(@"\begin{array}{c}");

                    // Add free variables:
                    Dictionary<string, Term[]> freeVarialesToTheirExpressions = new Dictionary<string, Term[]>();
                    for (int i = 0; i < ls.Equations.Count; i++)
                    {
                        Term leadingTerm = ls.Equations[i].GetLeadingTerm();
                        if (leadingTerm != null)
                        {
                            string leadingVar = leadingTerm.Variable;
                            latexBuf.Append($"{leadingVar} = ");
                            Term[] expressed = ls.Equations[i].ExpressVariable(leadingVar);

                            if (expressed.Length > 0)
                            {
                                var term = expressed[0];
                                string sign = term.IsPositive() ? "" : "-";
                                latexBuf.Append($"{sign} {term.Coefficient.Abs()} {term.Variable}");
                                for (int j = 1; j < expressed.Length; j++)
                                {
                                    term = expressed[j];
                                    sign = term.IsPositive() ? "+" : "-";
                                    latexBuf.Append($"{sign} {term.Coefficient.Abs()} {term.Variable}");
                                }
                            }
                            else
                            {
                                latexBuf.Append($"0");
                            }

                            latexBuf.AppendLine(@"\\");
                            freeVarialesToTheirExpressions.Add(leadingVar, expressed);
                        }
                    }

                    // Add non-free
                    List<string> notFreeVars = new List<string>();
                    foreach (LinearEquation v in ls.Equations)
                    {
                        foreach (Term t in v.GetTerms().Skip(1))
                        {
                            if (!freeVarialesToTheirExpressions.ContainsKey(t.Variable) && !notFreeVars.Contains(t.Variable))
                            {
                                if (!string.IsNullOrWhiteSpace(t.Variable))
                                {
                                    latexBuf.Append($"{t.Variable} = {t.Variable}");
                                    latexBuf.AppendLine(@"\\");
                                }
                                notFreeVars.Add(t.Variable);
                            }
                        }
                    }

                    latexBuf.Append(@"\end{array}");
                    latexBuf.AppendLine(@"\\");

                    IEnumerable<string> notFreeVarsNoConstant = notFreeVars.Where(x => !string.IsNullOrWhiteSpace(x));
                    List<string> uniqueVars = new List<string>(freeVarialesToTheirExpressions.Count() + notFreeVars.Count());
                    uniqueVars.AddRange(freeVarialesToTheirExpressions.Keys);
                    uniqueVars.AddRange(notFreeVarsNoConstant);

                    latexBuf.AppendLine(@"\{");

                    for (int i = 0; i < notFreeVars.Count; i++)
                    {
                        string row = notFreeVars[i];
                        latexBuf.Append(@"\left( \begin{array}{c}");
                        // TODO: implement ??
                        latexBuf.Append(@"\end{array} \right)");
                        
                        if (row != "") latexBuf.Append($" \\cdot {row}");
                        if (i < notFreeVars.Count() - 1) latexBuf.Append($" + ");

                        latexBuf.AppendLine();
                    }

                    //for (int j = 0; j < freeVarsExpressed.Count; j++)
                    //{
                    //    (string, Term[]) fvex = freeVarsExpressed[j];
                    //    string variable = fvex.Item1;

                    //    // TODO: implement this:
                    //    latexBuf.Append(@"\left( \begin{array}{c} -4 \\ 0 \\ 0 \end{array} \right)");
                    //    if (variable != "") latexBuf.Append($" \\cdot {variable}");
                    //    if (j < freeVarsExpressed.Count - 1) latexBuf.Append($" + ");
                    //}

                    latexBuf.AppendLine($" \\ | \\ {string.Join(",", notFreeVarsNoConstant)} \\in \\mathbb{{R}}");
                    latexBuf.AppendLine(@"\}");
                }
            }
            else
            {
                latexBuf.Append(@"\color{red}{Contradiction Found !}");
            }
        }
    }
}
