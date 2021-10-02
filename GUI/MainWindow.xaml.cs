using System;
using System.Windows;
using System.Windows.Media.Imaging;
using CSharpMath.SkiaSharp;
using System.IO;
using LinearEquationSolver.Parsers;
using LinearEquationSolver;
using System.Text;
using System.Linq;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
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

            // Sanitize user input
            userInput = userInput.Replace("−", "-");

            // Make equation out of user input:
            ILinearEquationParser lep = new BasicLinearEquationParser();
            string[] userSplit = userInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            LinearSystem ls = new LinearSystem();
            foreach (string rawEq in userSplit)
            {
                LinearEquation eq = lep.Parse(rawEq);
                ls.AddEquation(eq);
            }

            // Parse system solution to latex output:
            StringBuilder latexBuf = new StringBuilder();
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
                    // TODO: Represent this case as a parameterized solution.
                }
            }
            else
            {
                latexBuf.Append(@"\color{red}{Contradiction Found !}");
            }

            // Create latex bitmap out of it:
            string latex = latexBuf.ToString();
            var painter = new MathPainter();
            painter.LaTeX = latex;
            Stream bitmapStream = painter.DrawAsStream();
            
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = bitmapStream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();

            imgDynamic.Source = bitmap;
            imgDynamic.Width = bitmap.Width;
            imgDynamic.Height = bitmap.Height;
        }
    }
}
