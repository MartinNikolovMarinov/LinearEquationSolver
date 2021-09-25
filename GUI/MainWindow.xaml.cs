using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

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

            //BitmapImage src = new BitmapImage();
            //src.BeginInit();
            //src.UriSource = new Uri("picture.jpg", UriKind.Relative);
            //src.CacheOption = BitmapCacheOption.OnLoad;
            //src.EndInit();
        }

        private void BtnLoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Uri fileUri = new Uri(openFileDialog.FileName);
                imgDynamic.Source = new BitmapImage(fileUri);
            }
        }

        //private void BtnLoadFromResource_Click(object sender, RoutedEventArgs e)
        //{
        //    Uri resourceUri = new Uri("/Images/white_bengal_tiger.jpg", UriKind.Relative);
        //    imgDynamic.Source = new BitmapImage(resourceUri);
        //}
    }
}
