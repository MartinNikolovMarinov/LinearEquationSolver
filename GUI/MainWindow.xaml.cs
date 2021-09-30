using System;
using System.Windows;
using System.Windows.Media.Imaging;
using CSharpMath.SkiaSharp;
using System.IO;
using LinearEquationSolver.Parsers;
using LinearEquationSolver;
using System.Text;

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

            //string userInput = @"
            //    \begin{array}{rcc}
            //            2x - 2y & = & 0 \\
            //            z + 3w  & = & 2 \\
            //            3x + 3y & = & 0 \\ 
            //            x - y + 2z + 6w & = & 4
            //    \end{array}

            //    \quad \quad \quad \quad
            //    \frac{1}{2}p_{2} + p_{3}
            //    \longrightarrow
            //    \quad

            //    \begin{array}{rcc}
            //            2x - 2y & = & 0 \\
            //            z + 3w  & = & 2 \\
            //            3x + 3y & = & 0 \\ 
            //            x - y + 2z + 6w & = & 4
            //    \end{array}

            //    \quad \quad \quad \quad
            //    \frac{1}{2}p_{2} + p_{3}
            //    \longrightarrow
            //    \quad

            //    \begin{array}{rcc}
            //            2x - 2y & = & 0 \\
            //            z + 3w  & = & 2 \\
            //            3x + 3y & = & 0 \\ 
            //            x - y + 2z + 6w & = & 4
            //    \end{array}

            //    \quad \quad \quad \quad
            //    \frac{1}{2}p_{2} + p_{3}
            //    \longrightarrow
            //    \quad

            //    \begin{array}{rcc}
            //            2x - 2y & = & 0 \\
            //            z + 3w  & = & 2 \\
            //            3x + 3y & = & 0 \\ 
            //            x - y + 2z + 6w & = & 4
            //    \end{array}
            //";

            var painter = new MathPainter();
            ILinearEquationParser lep = new BasicLinearEquationParser();
            string[] userSplit = userInput.Split(Environment.NewLine);
            LinearSystem ls = new LinearSystem();
            foreach (string rawEq in userSplit)
            {
                LinearEquation eq = lep.Parse(rawEq);
                ls.AddEquation(eq);
            }

            ls.Solve();

            StringBuilder sb = new StringBuilder();
            foreach (LinearEquation eq in ls.Equations)
            {
                sb.Append(eq.ToString());
                sb.Append(@"\\");
            }

            painter.LaTeX = sb.ToString();
            Stream pngStream = painter.DrawAsStream();
            
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = pngStream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();

            imgDynamic.Source = bitmap;
            imgDynamic.Width = bitmap.Width;
            imgDynamic.Height = bitmap.Height;
        }
    }
}
