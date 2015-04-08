using ExifWriter.Model;

namespace Services.Exif
{
    public interface IExifService
    {
        void WriteAllExif(string filename, string exposuretime = null, float aperture = 0, float focalLength = 0,
            int iso = -1,
            string copyright = null);
        
        void WriteExposureTimeExif(string filename, string exposuretime);
        
        void WriteApertureExif(string filename, float aperture);
        
        void WriteFocalLengthExif(string filename, float focalLength);
        
        void WriteIsoExif(string filename, int iso);
        
        void WriteCopyrightExif(string filename, string copyright);
        
        ImageExifData ReadImageExifData(string filename);
    }
}