using System.Windows;
using ExifWriter.Application.Properties;
using ExifWriter.Application.Views;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.Unity;
using Services.Exif;

namespace ExifWriter.Application
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public IUnityContainer Container = new UnityContainer();

        protected override void OnStartup(StartupEventArgs e)
        {
            //service registrations
            Container.RegisterType<IExifService, ExifService>();

            //registraions utils
            //only one instance from messenger can exists! (recipient problems..)
            var messenger = new Messenger();
            Container.RegisterInstance(typeof (IMessenger), messenger);

            var mainWindow = Container.Resolve<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Settings.Default.Save();

            base.OnExit(e);
        }
    }
}