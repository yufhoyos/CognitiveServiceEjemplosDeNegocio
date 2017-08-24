using Microsoft.Practices.Unity;
using Prism.Unity;
using FaceApiAPP.Views;
using System.Windows;

namespace FaceApiAPP
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<FaceDetectWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }
    }
}
