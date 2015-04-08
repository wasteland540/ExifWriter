using System;
using System.Diagnostics;
using System.IO;
using ExifWriter.Model;

namespace Services.Exif
{
    public class ExifService : IExifService
    {
        private const string ExiftoolFilename = "Exif/exiftool.exe";
        private const string FocalLengthTagParam = " -exif:FocalLength";
        private const string ExposureTimeTagParam = " -exif:ExposureTime";
        private const string IsoTagParam = " -exif:ISO";
        //private const string IsoSpeedTagParam = " -exif:ISOSpeed";
        private const string FNumberTagParam = " -exif:FNumber";
        private const string CopyrightTagParam = " -exif:Copyright";

        public void WriteAllExif(string filename, string exposuretime = null, float aperture = 0.0f,
            float focalLength = 0.0f,
            int iso = -1,
            string copyright = null)
        {
            ProcessStartInfo exiftoolProcess = SetupExiftoolProcess();

            SetExposureTime(exposuretime, exiftoolProcess);
            SetAperture(aperture, exiftoolProcess);
            SetFocalLength(focalLength, exiftoolProcess);
            SetIso(iso, exiftoolProcess);
            SetCopyright(copyright, exiftoolProcess);

            StartProcess(filename, exiftoolProcess);
        }

        public void WriteExposureTimeExif(string filename, string exposuretime)
        {
            ProcessStartInfo exiftoolProcess = SetupExiftoolProcess();

            SetExposureTime(exposuretime, exiftoolProcess);

            StartProcess(filename, exiftoolProcess);
        }

        public void WriteApertureExif(string filename, float aperture)
        {
            ProcessStartInfo exiftoolProcess = SetupExiftoolProcess();

            SetAperture(aperture, exiftoolProcess);

            StartProcess(filename, exiftoolProcess);
        }

        public void WriteFocalLengthExif(string filename, float focalLength)
        {
            ProcessStartInfo exiftoolProcess = SetupExiftoolProcess();

            SetFocalLength(focalLength, exiftoolProcess);

            StartProcess(filename, exiftoolProcess);
        }

        public void WriteIsoExif(string filename, int iso)
        {
            ProcessStartInfo exiftoolProcess = SetupExiftoolProcess();

            SetIso(iso, exiftoolProcess);

            StartProcess(filename, exiftoolProcess);
        }

        public void WriteCopyrightExif(string filename, string copyright)
        {
            ProcessStartInfo exiftoolProcess = SetupExiftoolProcess();

            SetCopyright(copyright, exiftoolProcess);

            StartProcess(filename, exiftoolProcess);
        }

        public ImageExifData ReadImageExifData(string filename)
        {
            ProcessStartInfo exiftoolProcess = SetupExiftoolProcess();
            exiftoolProcess.Arguments += FocalLengthTagParam;
            exiftoolProcess.Arguments += ExposureTimeTagParam;
            exiftoolProcess.Arguments += IsoTagParam;
            exiftoolProcess.Arguments += FNumberTagParam;
            exiftoolProcess.Arguments += CopyrightTagParam;
            exiftoolProcess.Arguments += " \"" + filename + "\"";

            Process process = StartProcess(filename, exiftoolProcess);

            string outputFocalLength;
            string outputExposureTime;
            string outputIso;
            string outputFNumber;
            string outputCopyright;

            using (StreamReader stream = process.StandardOutput)
            {
                stream.ReadLine(); //skip filename line
                outputFocalLength = stream.ReadLine();
                outputExposureTime = stream.ReadLine();
                outputIso = stream.ReadLine();
                outputFNumber = stream.ReadLine();
                outputCopyright = stream.ReadLine();

                stream.Close();
            }

            float aperture = float.NaN;
            float focalLength = float.NaN;
            string exposureTime = string.Empty;
            int iso = int.MinValue;
            string copyright = string.Empty;

            if (!string.IsNullOrEmpty(outputFNumber))
            {
                string buff = GetValueFromOutputString(outputFNumber);
                buff = buff.Replace('.', ',');

                try
                {
                    aperture = float.Parse(buff);
                }
                    // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception)
                {
                }
            }

            if (!string.IsNullOrEmpty(outputFocalLength))
            {
                string buff = GetValueFromOutputString(outputFocalLength);
                buff = buff.Replace("mm", "").Trim();
                buff = buff.Replace('.', ',');

                try
                {
                    focalLength = float.Parse(buff);
                }
                    // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception)
                {
                }
            }

            if (!string.IsNullOrEmpty(outputExposureTime))
            {
                exposureTime = GetValueFromOutputString(outputExposureTime);
            }

            if (!string.IsNullOrEmpty(outputIso))
            {
                string buff = GetValueFromOutputString(outputIso);

                try
                {
                    iso = int.Parse(buff);
                }
                    // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception)
                {
                }
            }

            if (!string.IsNullOrEmpty(outputCopyright))
            {
                copyright = GetValueFromOutputString(outputCopyright);
            }

            return new ImageExifData
            {
                Aperture = aperture,
                ExposureTime = exposureTime,
                FocalLength = focalLength,
                Iso = iso,
                Copyright = copyright
            };
        }

        private static ProcessStartInfo SetupExiftoolProcess()
        {
            var exiftoolProcess = new ProcessStartInfo(Path.Combine(Environment.CurrentDirectory, ExiftoolFilename))
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            return exiftoolProcess;
        }

        private static Process StartProcess(string filename, ProcessStartInfo exiftoolProcess)
        {
            exiftoolProcess.Arguments += " -overwrite_original_in_place";
            exiftoolProcess.Arguments += " \"" + filename + "\"";

            var process = new Process {StartInfo = exiftoolProcess};
            process.Start();

            return process;
        }

        private static void SetCopyright(string copyright, ProcessStartInfo exiftoolProcess)
        {
            if (!string.IsNullOrEmpty(copyright))
            {
                SetParam(exiftoolProcess, CopyrightTagParam, copyright);
            }
        }

        private static void SetIso(int iso, ProcessStartInfo exiftoolProcess)
        {
            if (iso > 0)
            {
                SetParam(exiftoolProcess, IsoTagParam, iso);
                //SetParam(exiftoolProcess, IsoSpeedTagParam, iso);
            }
        }

        private static void SetFocalLength(float focalLength, ProcessStartInfo exiftoolProcess)
        {
            if (focalLength > 0.0f)
            {
                SetParam(exiftoolProcess, FocalLengthTagParam, focalLength);
            }
        }

        private static void SetAperture(float aperture, ProcessStartInfo exiftoolProcess)
        {
            if (aperture > 0.0f)
            {
                SetParam(exiftoolProcess, FNumberTagParam, aperture);
            }
        }

        private static void SetExposureTime(string exposuretime, ProcessStartInfo exiftoolProcess)
        {
            if (!string.IsNullOrEmpty(exposuretime))
            {
                double exposureTime = DetermineExpositionTime(exposuretime);

                if (!double.IsNaN(exposureTime))
                {
                    SetParam(exiftoolProcess, ExposureTimeTagParam, exposuretime);
                }
            }
        }

        private static void SetParam(ProcessStartInfo exiftoolProcess, string param, object value)
        {
            exiftoolProcess.Arguments += param;
            exiftoolProcess.Arguments += "=";

            if (value is string)
            {
                exiftoolProcess.Arguments += "\"" + value + "\"";
            }
            else
            {
                exiftoolProcess.Arguments += value;
            }
        }

        private static double DetermineExpositionTime(string exposionTimeString)
        {
            double expositionTime = Double.NaN;

            if (exposionTimeString.Contains("/"))
            {
                string dividendBuff = exposionTimeString.Substring(0,
                    exposionTimeString.IndexOf("/", StringComparison.Ordinal));
                double dividend = double.Parse(dividendBuff);

                string divisorBuff =
                    exposionTimeString.Substring(exposionTimeString.IndexOf("/", StringComparison.Ordinal) + 1);
                double divisor = double.Parse(divisorBuff);

                // ReSharper disable once PossibleLossOfFraction
                expositionTime = dividend/divisor;
            }
            else
            {
                try
                {
                    expositionTime = double.Parse(exposionTimeString);
                }
// ReSharper disable once EmptyGeneralCatchClause
                catch (Exception)
                {
                }
            }

            return expositionTime;
        }

        private static string GetValueFromOutputString(string outputString)
        {
            string result = outputString.Trim();

            result = result.IndexOf(":", StringComparison.Ordinal) + 2 < result.Length
                ? result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 2)
                : string.Empty;

            return result;
        }
    }
}