using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows;

namespace FaceApiAPP.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FaceDetectWindow : Window
    {
        public FaceDetectWindow()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            mediaUriElement.Stop();
        }
    }
}
