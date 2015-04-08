using System;
using System.Threading;
using ExifWriter.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Exif;

namespace ServicesUnitTest.Exif
{
    /// <summary>
    ///     Summary description for ExifServiceUnitTest
    /// </summary>
    [TestClass]
    public class ExifServiceUnitTest
    {
        //canon raw for read test
        private const string AFshortInput =
            @"C:\Users\m.elz.KS21\Documents\Visual Studio 2013\Projects\ExifWriter\ServicesUnitTest\Resources\Exif\Input\B4.5-30mm.CR2";

        private const string AFlongInput =
            @"C:\Users\m.elz.KS21\Documents\Visual Studio 2013\Projects\ExifWriter\ServicesUnitTest\Resources\Exif\Input\B11-30mm.CR2";

        private const string MFshortInput =
            @"C:\Users\m.elz.KS21\Documents\Visual Studio 2013\Projects\ExifWriter\ServicesUnitTest\Resources\Exif\Input\B4-28mm.CR2";

        private const string MFlongInput =
            @"C:\Users\m.elz.KS21\Documents\Visual Studio 2013\Projects\ExifWriter\ServicesUnitTest\Resources\Exif\Input\B8-28mm.CR2";

        //jpg for read test
        private const string JpgInput =
            @"C:\Users\m.elz.KS21\Documents\Visual Studio 2013\Projects\ExifWriter\ServicesUnitTest\Resources\Exif\Input\B4-28mm.jpg";

        //canon raw for write tests
        private const string WriteAllInput =
            @"C:\Users\m.elz.KS21\Documents\Visual Studio 2013\Projects\ExifWriter\ServicesUnitTest\Resources\Exif\InputWrite\WriteAll.CR2";

        private const string WriteExposureTimeInput =
            @"C:\Users\m.elz.KS21\Documents\Visual Studio 2013\Projects\ExifWriter\ServicesUnitTest\Resources\Exif\InputWrite\WriteExposureTime.CR2";

        private const string WriteApertureInput =
            @"C:\Users\m.elz.KS21\Documents\Visual Studio 2013\Projects\ExifWriter\ServicesUnitTest\Resources\Exif\InputWrite\WriteAperture.CR2";

        private const string WriteFocalLengthInput =
            @"C:\Users\m.elz.KS21\Documents\Visual Studio 2013\Projects\ExifWriter\ServicesUnitTest\Resources\Exif\InputWrite\WriteFocalLength.CR2";

        private const string WriteIsoInput =
            @"C:\Users\m.elz.KS21\Documents\Visual Studio 2013\Projects\ExifWriter\ServicesUnitTest\Resources\Exif\InputWrite\WriteIso.CR2";

        private const string WriteCopyrightInput =
            @"C:\Users\m.elz.KS21\Documents\Visual Studio 2013\Projects\ExifWriter\ServicesUnitTest\Resources\Exif\InputWrite\WriteCopyright.CR2";

        //jpg for write test
        private const string WriteAllJpgInput =
            @"C:\Users\m.elz.KS21\Documents\Visual Studio 2013\Projects\ExifWriter\ServicesUnitTest\Resources\Exif\InputWrite\WriteAll.jpg";

        private const string WriteExposureTimeJpgInput =
            @"C:\Users\m.elz.KS21\Documents\Visual Studio 2013\Projects\ExifWriter\ServicesUnitTest\Resources\Exif\InputWrite\WriteExposureTime.jpg";

        private const string WriteApertureJpgInput =
            @"C:\Users\m.elz.KS21\Documents\Visual Studio 2013\Projects\ExifWriter\ServicesUnitTest\Resources\Exif\InputWrite\WriteAperture.jpg";

        private const string WriteFocalLengthJpgInput =
            @"C:\Users\m.elz.KS21\Documents\Visual Studio 2013\Projects\ExifWriter\ServicesUnitTest\Resources\Exif\InputWrite\WriteFocalLength.jpg";

        private const string WriteIsoJpgInput =
            @"C:\Users\m.elz.KS21\Documents\Visual Studio 2013\Projects\ExifWriter\ServicesUnitTest\Resources\Exif\InputWrite\WriteIso.jpg";

        private const string WriteCopyrightJpgInput =
            @"C:\Users\m.elz.KS21\Documents\Visual Studio 2013\Projects\ExifWriter\ServicesUnitTest\Resources\Exif\InputWrite\WriteCopyright.jpg";

        private IExifService _service;

        [TestInitialize]
        public void Setup()
        {
            _service = new ExifService();
        }

        [TestMethod]
        public void ReadImageExifData()
        {
            ImageExifData afShort = _service.ReadImageExifData(AFshortInput);

            Assert.IsTrue(Math.Abs(afShort.Aperture - 4.5) < 0.00001);
            Assert.IsTrue(string.IsNullOrEmpty(afShort.Copyright));
            Assert.IsTrue(afShort.ExposureTime == "1/50");
            Assert.IsTrue(Math.Abs(afShort.FocalLength - 30.0) < 0.00001);
            Assert.IsTrue(afShort.Iso == 3200);


            ImageExifData afLong = _service.ReadImageExifData(AFlongInput);

            Assert.IsTrue(Math.Abs(afLong.Aperture - 11.0) < 0.00001);
            Assert.IsTrue(string.IsNullOrEmpty(afLong.Copyright));
            Assert.IsTrue(afLong.ExposureTime == "10");
            Assert.IsTrue(Math.Abs(afLong.FocalLength - 30.0) < 0.00001);
            Assert.IsTrue(afLong.Iso == 100);


            ImageExifData mfShort = _service.ReadImageExifData(MFshortInput);

            Assert.IsTrue(Math.Abs(mfShort.Aperture - 0.0) < 0.00001);
            Assert.IsTrue(string.IsNullOrEmpty(mfShort.Copyright));
            Assert.IsTrue(mfShort.ExposureTime == "1/50");
            Assert.IsTrue(Math.Abs(mfShort.FocalLength - 50.0) < 0.00001);
            Assert.IsTrue(mfShort.Iso == 3200);


            ImageExifData mfLong = _service.ReadImageExifData(MFlongInput);

            Assert.IsTrue(Math.Abs(mfLong.Aperture - 0.0) < 0.00001);
            Assert.IsTrue(string.IsNullOrEmpty(mfLong.Copyright));
            Assert.IsTrue(mfLong.ExposureTime == "4");
            Assert.IsTrue(Math.Abs(mfLong.FocalLength - 50.0) < 0.00001);
            Assert.IsTrue(mfLong.Iso == 100);


            ImageExifData exifDataJpg = _service.ReadImageExifData(JpgInput);

            Assert.IsTrue(Math.Abs(exifDataJpg.Aperture - 4.0) < 0.1);
            Assert.IsTrue(exifDataJpg.Copyright == "m.elz");
            Assert.IsTrue(exifDataJpg.ExposureTime == "4");
            Assert.IsTrue(Math.Abs(exifDataJpg.FocalLength - 28.0) < 0.00001);
            Assert.IsTrue(exifDataJpg.Iso == 100);
        }

        [TestMethod]
        public void WriteAllExif()
        {
            ImageExifData exifData = _service.ReadImageExifData(WriteAllInput);

            exifData.Aperture = 4.0f;
            exifData.Copyright = "m.elz";
            exifData.ExposureTime = "4";
            exifData.FocalLength = 28.0f;
            exifData.Iso = 100;

            _service.WriteAllExif(WriteAllInput, exifData.ExposureTime, exifData.Aperture, exifData.FocalLength,
                exifData.Iso, exifData.Copyright);
            Thread.Sleep(2000);
            exifData = _service.ReadImageExifData(WriteAllInput);

            Assert.IsTrue(Math.Abs(exifData.Aperture - 4.0) < 0.1);
            Assert.IsTrue(exifData.Copyright == "m.elz");
            Assert.IsTrue(exifData.ExposureTime == "4");
            Assert.IsTrue(Math.Abs(exifData.FocalLength - 28.0) < 0.00001);
            Assert.IsTrue(exifData.Iso == 100);


            ImageExifData exifDataJpg = _service.ReadImageExifData(WriteAllJpgInput);

            exifDataJpg.Aperture = 4.0f;
            exifDataJpg.Copyright = "m.elz";
            exifDataJpg.ExposureTime = "4";
            exifDataJpg.FocalLength = 28.0f;
            exifDataJpg.Iso = 100;

            _service.WriteAllExif(WriteAllJpgInput, exifDataJpg.ExposureTime, exifDataJpg.Aperture,
                exifDataJpg.FocalLength, exifDataJpg.Iso, exifDataJpg.Copyright);
            Thread.Sleep(2000);
            exifDataJpg = _service.ReadImageExifData(WriteAllJpgInput);

            Assert.IsTrue(Math.Abs(exifDataJpg.Aperture - 4.0) < 0.1);
            Assert.IsTrue(exifDataJpg.Copyright == "m.elz");
            Assert.IsTrue(exifDataJpg.ExposureTime == "4");
            Assert.IsTrue(Math.Abs(exifDataJpg.FocalLength - 28.0) < 0.00001);
            Assert.IsTrue(exifDataJpg.Iso == 100);
        }

        [TestMethod]
        public void WriteExposureTimeExif()
        {
            _service.WriteExposureTimeExif(WriteExposureTimeInput, "1/250");
            Thread.Sleep(2000);
            ImageExifData exifData = _service.ReadImageExifData(WriteExposureTimeInput);

            Assert.IsTrue(Math.Abs(exifData.Aperture - 4.0) < 0.1);
            Assert.IsTrue(exifData.Copyright == "m.elz");
            Assert.IsTrue(exifData.ExposureTime == "1/250");
            Assert.IsTrue(Math.Abs(exifData.FocalLength - 28.0) < 0.00001);
            Assert.IsTrue(exifData.Iso == 100);


            _service.WriteExposureTimeExif(WriteExposureTimeJpgInput, "1/250");
            Thread.Sleep(2000);
            ImageExifData exifDataJpg = _service.ReadImageExifData(WriteExposureTimeJpgInput);

            Assert.IsTrue(Math.Abs(exifDataJpg.Aperture - 4.0) < 0.1);
            Assert.IsTrue(exifDataJpg.Copyright == "m.elz");
            Assert.IsTrue(exifDataJpg.ExposureTime == "1/250");
            Assert.IsTrue(Math.Abs(exifDataJpg.FocalLength - 28.0) < 0.00001);
            Assert.IsTrue(exifDataJpg.Iso == 100);
        }

        [TestMethod]
        public void WriteApertureExif()
        {
            _service.WriteApertureExif(WriteApertureInput, 11.0f);
            Thread.Sleep(2000);
            ImageExifData exifData = _service.ReadImageExifData(WriteApertureInput);

            Assert.IsTrue(Math.Abs(exifData.Aperture - 11.0) < 0.1);
            Assert.IsTrue(exifData.Copyright == "m.elz");
            Assert.IsTrue(exifData.ExposureTime == "4");
            Assert.IsTrue(Math.Abs(exifData.FocalLength - 28.0) < 0.00001);
            Assert.IsTrue(exifData.Iso == 100);

            _service.WriteApertureExif(WriteApertureJpgInput, 11.0f);
            Thread.Sleep(2000);
            ImageExifData exifDataJpg = _service.ReadImageExifData(WriteApertureJpgInput);

            Assert.IsTrue(Math.Abs(exifDataJpg.Aperture - 11.0) < 0.1);
            Assert.IsTrue(exifDataJpg.Copyright == "m.elz");
            Assert.IsTrue(exifDataJpg.ExposureTime == "4");
            Assert.IsTrue(Math.Abs(exifDataJpg.FocalLength - 28.0) < 0.00001);
            Assert.IsTrue(exifDataJpg.Iso == 100);
        }

        [TestMethod]
        public void WriteFocalLengthExif()
        {
            _service.WriteFocalLengthExif(WriteFocalLengthInput, 50.0f);
            Thread.Sleep(2000);
            ImageExifData exifData = _service.ReadImageExifData(WriteFocalLengthInput);

            Assert.IsTrue(Math.Abs(exifData.Aperture - 4.0) < 0.1);
            Assert.IsTrue(exifData.Copyright == "m.elz");
            Assert.IsTrue(exifData.ExposureTime == "4");
            Assert.IsTrue(Math.Abs(exifData.FocalLength - 50.0) < 0.00001);
            Assert.IsTrue(exifData.Iso == 100);


            _service.WriteFocalLengthExif(WriteFocalLengthJpgInput, 50.0f);
            Thread.Sleep(2000);
            ImageExifData exifDataJpg = _service.ReadImageExifData(WriteFocalLengthJpgInput);

            Assert.IsTrue(Math.Abs(exifDataJpg.Aperture - 4.0) < 0.1);
            Assert.IsTrue(exifDataJpg.Copyright == "m.elz");
            Assert.IsTrue(exifDataJpg.ExposureTime == "4");
            Assert.IsTrue(Math.Abs(exifDataJpg.FocalLength - 50.0) < 0.00001);
            Assert.IsTrue(exifDataJpg.Iso == 100);
        }

        [TestMethod]
        public void WriteIsoExif()
        {
            _service.WriteIsoExif(WriteIsoInput, 800);
            Thread.Sleep(2000);
            ImageExifData exifData = _service.ReadImageExifData(WriteIsoInput);

            Assert.IsTrue(Math.Abs(exifData.Aperture - 4.0) < 0.1);
            Assert.IsTrue(exifData.Copyright == "m.elz");
            Assert.IsTrue(exifData.ExposureTime == "4");
            Assert.IsTrue(Math.Abs(exifData.FocalLength - 28.0) < 0.00001);
            Assert.IsTrue(exifData.Iso == 800);


            _service.WriteIsoExif(WriteIsoJpgInput, 800);
            Thread.Sleep(2000);
            ImageExifData exifDataJpg = _service.ReadImageExifData(WriteIsoJpgInput);

            Assert.IsTrue(Math.Abs(exifDataJpg.Aperture - 4.0) < 0.1);
            Assert.IsTrue(exifDataJpg.Copyright == "m.elz");
            Assert.IsTrue(exifDataJpg.ExposureTime == "4");
            Assert.IsTrue(Math.Abs(exifDataJpg.FocalLength - 28.0) < 0.00001);
            Assert.IsTrue(exifDataJpg.Iso == 800);
        }

        [TestMethod]
        public void WriteCopyrightExif()
        {
            _service.WriteCopyrightExif(WriteCopyrightInput, "Exif Writer by m.elz");
            Thread.Sleep(2000);
            ImageExifData exifData = _service.ReadImageExifData(WriteCopyrightInput);

            Assert.IsTrue(Math.Abs(exifData.Aperture - 4.0) < 0.1);
            Assert.IsTrue(exifData.Copyright == "Exif Writer by m.elz");
            Assert.IsTrue(exifData.ExposureTime == "4");
            Assert.IsTrue(Math.Abs(exifData.FocalLength - 28.0) < 0.00001);
            Assert.IsTrue(exifData.Iso == 100);


            _service.WriteCopyrightExif(WriteCopyrightJpgInput, "Exif Writer by m.elz");
            Thread.Sleep(2000);
            ImageExifData exifDataJpg = _service.ReadImageExifData(WriteCopyrightJpgInput);

            Assert.IsTrue(Math.Abs(exifDataJpg.Aperture - 4.0) < 0.1);
            Assert.IsTrue(exifDataJpg.Copyright == "Exif Writer by m.elz");
            Assert.IsTrue(exifDataJpg.ExposureTime == "4");
            Assert.IsTrue(Math.Abs(exifDataJpg.FocalLength - 28.0) < 0.00001);
            Assert.IsTrue(exifDataJpg.Iso == 100);
        }
    }
}