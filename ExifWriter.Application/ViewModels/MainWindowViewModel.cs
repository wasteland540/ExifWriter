using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Commands.Commands;
using ExifWriter.Application.Properties;
using ExifWriter.Model;
using Microsoft.Win32;
using Services.Exif;

namespace ExifWriter.Application.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const string PlaceholderPath = "Resources/Placeholders/preview.png";
        private const string PlaceholderProgressLabel = "Nothing to do...";
        private const string LoadingProgressLabel = "Loading files...";
        private const string SynchronizeProgressLabel = "Synchronize files...";
        private const string SavingProgressLabel = "Saving files...";
        private readonly IExifService _exifService;

        private bool _apertureSyncable = Settings.Default.ApertureSyncable;
        private bool _backupOriginals = Settings.Default.Config_BackupOriginal;
        private BitmapImage _bitmapPreview;
        private bool _copyrightSyncable = Settings.Default.CopyrightSyncable;
        private ICommand _exitCommand;
        private bool _exposureTimeSyncable = Settings.Default.ExposureTimeSyncable;
        private bool _focalLengthSyncable = Settings.Default.FocalLengthSyncable;
        private List<ImageExifData> _imageExifDataList = new List<ImageExifData>();
        private bool _isoSyncable = Settings.Default.IsoSyncable;

        private ICommand _openCommand;
        private ICommand _openSequenzCommand;
        private string _progressLabel = PlaceholderProgressLabel;
        private int _progressState;
        private ICommand _saveCommand;

        private ImageExifData _selectedImage;
        private string _selectedPicturePath;
        private ICommand _syncCommand;

        public MainWindowViewModel(IExifService exifService)
        {
            _exifService = exifService;
        }

        #region Properties

        public bool ExposureTimeSyncable
        {
            get { return _exposureTimeSyncable; }
            set
            {
                if (value != _exposureTimeSyncable)
                {
                    _exposureTimeSyncable = value;
                    Settings.Default.ExposureTimeSyncable = _exposureTimeSyncable;

                    RaisePropertyChanged("ExposureTimeSyncable");
                }
            }
        }

        public bool ApertureSyncable
        {
            get { return _apertureSyncable; }
            set
            {
                if (value != _apertureSyncable)
                {
                    _apertureSyncable = value;
                    Settings.Default.ApertureSyncable = _apertureSyncable;

                    RaisePropertyChanged("ApertureSyncable");
                }
            }
        }

        public bool FocalLengthSyncable
        {
            get { return _focalLengthSyncable; }
            set
            {
                if (value != _focalLengthSyncable)
                {
                    _focalLengthSyncable = value;
                    Settings.Default.FocalLengthSyncable = _focalLengthSyncable;

                    RaisePropertyChanged("FocalLengthSyncable");
                }
            }
        }

        public bool IsoSyncable
        {
            get { return _isoSyncable; }
            set
            {
                if (value != _isoSyncable)
                {
                    _isoSyncable = value;
                    Settings.Default.IsoSyncable = _isoSyncable;

                    RaisePropertyChanged("IsoSyncable");
                }
            }
        }

        public bool CopyrightSyncable
        {
            get { return _copyrightSyncable; }
            set
            {
                if (value != _copyrightSyncable)
                {
                    _copyrightSyncable = value;
                    Settings.Default.CopyrightSyncable = _copyrightSyncable;

                    RaisePropertyChanged("CopyrightSyncable");
                }
            }
        }

        public bool BackupOriginals
        {
            get { return _backupOriginals; }
            set
            {
                if (value != _backupOriginals)
                {
                    _backupOriginals = value;
                    Settings.Default.Config_BackupOriginal = _backupOriginals;

                    RaisePropertyChanged("BackupOriginals");
                }
            }
        }

        public List<ImageExifData> ImageExifDataList
        {
            get { return _imageExifDataList; }
            set
            {
                if (value != null && value != _imageExifDataList)
                {
                    _imageExifDataList = value;

                    RaisePropertyChanged("ImageExifDataList");
                }
            }
        }

        public ICommand OpenCommand
        {
            get { return _openCommand = _openCommand ?? new DelegateCommand(Open); }
        }

        public ICommand OpenSequenzCommand
        {
            get { return _openSequenzCommand = _openSequenzCommand ?? new DelegateCommand(OpenSequenz); }
        }

        public ICommand SaveCommand
        {
            get { return _saveCommand = _saveCommand ?? new DelegateCommand(Save); }
        }

        public ICommand SyncCommand
        {
            get { return _syncCommand = _syncCommand ?? new DelegateCommand(SyncProperties); }
        }

        public ICommand ExitCommand
        {
            get { return _exitCommand = _exitCommand ?? new DelegateCommand(Exit); }
        }

        public ImageExifData SelectedImage
        {
            get { return _selectedImage; }
            set
            {
                if (value != null && value != _selectedImage)
                {
                    _selectedImage = value;
                    SelectedPicturePath = _selectedImage.Filename;

                    RaisePropertyChanged("SelectedImage");
                }
            }
        }

        public string SelectedPicturePath
        {
            get { return _selectedPicturePath; }
            set
            {
                if (value != null && value != _selectedPicturePath)
                {
                    LoadBitmapPreview(Path.Combine(Environment.CurrentDirectory, PlaceholderPath));

                    if (value != PlaceholderPath)
                    {
                        //it took some seconds to preview RAW files
                        new Thread(() =>
                        {
                            LoadBitmapPreview(value);

                            _selectedPicturePath = value;
                        }).Start();
                    }
                }
            }
        }

        public BitmapImage BitmapPreview
        {
            get
            {
                if (_bitmapPreview == null && string.IsNullOrEmpty(_selectedPicturePath))
                {
                    SelectedPicturePath = PlaceholderPath;
                }

                return _bitmapPreview;
            }
            set
            {
                if (value != null)
                {
                    _bitmapPreview = value;

                    RaisePropertyChanged("BitmapPreview");
                }
            }
        }

        public string ProgressLabel
        {
            get { return _progressLabel; }
            set
            {
                if (value != null && value != _progressLabel)
                {
                    _progressLabel = value;

                    RaisePropertyChanged("ProgressLabel");
                }
            }
        }

        public int ProgressState
        {
            get { return _progressState; }
            set
            {
                if (value != _progressState)
                {
                    _progressState = value;

                    RaisePropertyChanged("ProgressState");
                }
            }
        }

        #endregion Properties

        #region private methods

        private void Open(object obj)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter =
                    "JPG Files (.jpg)|*.jpg|JPEG Files (.jpeg)|*.jpeg|Canon RAW Files(.cr2)|*.cr2|All files (*.*)|*.*"
            };

            bool? dialogResult = openFileDialog.ShowDialog();

            if (dialogResult != null && dialogResult.Value)
            {
                ProgressLabel = LoadingProgressLabel;

                var backgroundWorker = new BackgroundWorker {WorkerReportsProgress = true};
                backgroundWorker.DoWork += backgroundWorker_Open;
                backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;

                backgroundWorker.RunWorkerAsync(openFileDialog.FileNames);
            }
        }

        private void OpenSequenz(object obj)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter =
                    "JPG Files (.jpg)|*.jpg|JPEG Files (.jpeg)|*.jpeg|Canon RAW Files(.cr2)|*.cr2|All files (*.*)|*.*"
            };

            bool? dialogResult = openFileDialog.ShowDialog();

            if (dialogResult != null && dialogResult.Value)
            {
                ProgressLabel = LoadingProgressLabel;

                var backgroundWorker = new BackgroundWorker {WorkerReportsProgress = true};
                backgroundWorker.DoWork += backgroundWorker_OpenSequenz;
                backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;

                backgroundWorker.RunWorkerAsync(openFileDialog.FileName);
            }
        }

        private void AddFile(string fileName, List<ImageExifData> imageList)
        {
            ImageExifData imageExifData = _exifService.ReadImageExifData(fileName);
            imageExifData.Filename = fileName;

            imageList.Add(imageExifData);
        }

        private void Save(object obj)
        {
            if (_imageExifDataList.Count > 0)
            {
                ProgressLabel = SavingProgressLabel;

                var backgroundWorker = new BackgroundWorker {WorkerReportsProgress = true};
                backgroundWorker.DoWork += backgroundWorker_Save;
                backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;

                backgroundWorker.RunWorkerAsync();
            }
        }

        private int BackupFilesIfConfigured(object sender, int progress, int progressStep)
        {
            if (_backupOriginals)
            {
                if (_imageExifDataList.Count > 0)
                {
                    string workingDir = Path.GetDirectoryName(_imageExifDataList[0].Filename);

                    if (workingDir != null)
                    {
                        string backupDirName = Path.Combine(workingDir, "BAK_" + DateTime.Now.Ticks);
                        Directory.CreateDirectory(backupDirName);

                        foreach (ImageExifData exifData in _imageExifDataList)
                        {
                            var fileInfo = new FileInfo(exifData.Filename);

                            string filename = Path.GetFileName(exifData.Filename);

                            if (filename != null)
                            {
                                fileInfo.CopyTo(Path.Combine(backupDirName, filename));
                            }

                            progress = UpdateProgressBar(sender, progress, progressStep);
                        }
                    }
                }
            }

            return progress;
        }

        private void SyncProperties(object obj)
        {
            if (_selectedImage != null && _imageExifDataList.Count > 0)
            {
                ProgressLabel = SynchronizeProgressLabel;

                var backgroundWorker = new BackgroundWorker {WorkerReportsProgress = true};
                backgroundWorker.DoWork += backgroundWorker_Sync;
                backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;

                backgroundWorker.RunWorkerAsync();
            }
        }

        private void Exit(object obj)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void LoadBitmapPreview(string value)
        {
            using (var stream = new FileStream(value, FileMode.Open, FileAccess.Read))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                BitmapPreview = bitmap;
            }
        }

        #endregion private methods

        #region backgroundworker

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressState = e.ProgressPercentage;
        }

        private static int UpdateProgressBar(object sender, int progress, int progressStep)
        {
            progress += progressStep;

            var backgroundWorker = sender as BackgroundWorker;

            if (backgroundWorker != null)
            {
                backgroundWorker.ReportProgress(progress);
            }

            return progress;
        }

        private void backgroundWorker_OpenSequenz(object sender, DoWorkEventArgs e)
        {
            var filenName = (string) e.Argument;
            var imageList = new List<ImageExifData>();

            string directory = Path.GetDirectoryName(filenName);

            if (directory != null)
            {
                string extension = Path.GetExtension(filenName);
                string[] files = Directory.GetFiles(directory, "*" + extension, SearchOption.TopDirectoryOnly);

                #region progressBar update

                int progressStep = 100/files.Length;
                int progress = 0;

                #endregion progressBar update

                foreach (string fileName in files)
                {
                    AddFile(fileName, imageList);

                    progress = UpdateProgressBar(sender, progress, progressStep);
                }
            }

            ImageExifDataList = imageList;
            ProgressLabel = PlaceholderProgressLabel;
            ProgressState = 0;
        }

        private void backgroundWorker_Open(object sender, DoWorkEventArgs e)
        {
            var filenNames = (string[]) e.Argument;
            var imageList = new List<ImageExifData>();

            #region progressBar update

            int progressStep = 100/filenNames.Length;
            int progress = 0;

            #endregion progressBar update

            foreach (string fileName in filenNames)
            {
                AddFile(fileName, imageList);

                progress = UpdateProgressBar(sender, progress, progressStep);
            }

            ImageExifDataList = imageList;
            ProgressLabel = PlaceholderProgressLabel;
            ProgressState = 0;
        }

        private void backgroundWorker_Save(object sender, DoWorkEventArgs e)
        {
            #region progressBar update

            int progressStep;

            if (_backupOriginals)
                progressStep = 100/(_imageExifDataList.Count*2);
            else
                progressStep = 100/_imageExifDataList.Count;

            int progress = 0;

            #endregion progressBar update

            progress = BackupFilesIfConfigured(sender, progress, progressStep);

            foreach (ImageExifData exifData in _imageExifDataList)
            {
                _exifService.WriteAllExif(exifData.Filename, exifData.ExposureTime, exifData.Aperture,
                    exifData.FocalLength, exifData.Iso, exifData.Copyright);

                progress = UpdateProgressBar(sender, progress, progressStep);
            }

            MessageBox.Show("All data saved!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            ProgressLabel = PlaceholderProgressLabel;
            ProgressState = 0;
        }

        private void backgroundWorker_Sync(object sender, DoWorkEventArgs e)
        {
            #region progressBar update

            int progressStep = 100/_imageExifDataList.Count;
            int progress = 0;

            #endregion progressBar update

            foreach (ImageExifData exifData in _imageExifDataList)
            {
                if (_selectedImage != null)
                {
                    if (ApertureSyncable)
                    {
                        exifData.Aperture = _selectedImage.Aperture;
                        exifData.RaisePropertyChanged("Aperture");
                    }

                    if (CopyrightSyncable)
                    {
                        exifData.Copyright = _selectedImage.Copyright;
                        exifData.RaisePropertyChanged("Copyright");
                    }

                    if (ExposureTimeSyncable)
                    {
                        exifData.ExposureTime = _selectedImage.ExposureTime;
                        exifData.RaisePropertyChanged("ExposureTime");
                    }

                    if (FocalLengthSyncable)
                    {
                        exifData.FocalLength = _selectedImage.FocalLength;
                        exifData.RaisePropertyChanged("FocalLength");
                    }

                    if (IsoSyncable)
                    {
                        exifData.Iso = _selectedImage.Iso;
                        exifData.RaisePropertyChanged("Iso");
                    }
                }

                progress = UpdateProgressBar(sender, progress, progressStep);
            }

            MessageBox.Show("All data synchronized!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            ProgressLabel = PlaceholderProgressLabel;
            ProgressState = 0;
        }

        #endregion backgroundworker
    }
}