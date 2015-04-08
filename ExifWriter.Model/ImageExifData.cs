using System.ComponentModel;

namespace ExifWriter.Model
{
    public class ImageExifData : INotifyPropertyChanged
    {
        public string Filename { get; set; }
        public string ExposureTime { get; set; }
        public float Aperture { get; set; }
        public float FocalLength { get; set; }
        public int Iso { get; set; }
        public string Copyright { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}