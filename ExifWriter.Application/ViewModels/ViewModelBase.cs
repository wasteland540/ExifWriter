using System.ComponentModel;
using Microsoft.Practices.Unity;

namespace ExifWriter.Application.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected IUnityContainer Container;

        protected ViewModelBase()
        {
            var app = (App) System.Windows.Application.Current;
            Container = app.Container;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
