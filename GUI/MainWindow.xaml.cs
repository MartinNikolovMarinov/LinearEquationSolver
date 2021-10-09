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
                    // TODO: parameterized solution
                }
            }
            else
            {
                latexBuf.Append(@"\color{red}{Contradiction Found !}");
            }
        }
    }
}
