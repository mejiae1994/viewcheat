using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace viewcheat
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<ImageModel> _images = new ObservableCollection<ImageModel>();

        public ObservableCollection<ImageModel> Images
        {
            get { return _images; }
            set
            {
                if (_images != value)
                {
                    _images = value;
                    OnPropertyChanged(nameof(Images));
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        private string _currentDir = "Loading Directory";
        public string currentDir
        {
            get { return _currentDir; }
            set
            {
                if (_currentDir != value)
                {
                    _currentDir = value;
                    OnPropertyChanged(nameof(currentDir));
                }
            }
        }

        private string _imageCount;
        public string imageCount
        {
            get { return _imageCount; }
            set
            {
                if (_imageCount != value)
                {
                    _imageCount = value;
                    OnPropertyChanged(nameof(imageCount));
                }
            }
        }

        private Visibility _listBoxVisibility;

        public Visibility listBoxVisibility
        {
            get { return _listBoxVisibility; }
            set
            {
                _listBoxVisibility = value;
                OnPropertyChanged(nameof(listBoxVisibility));
            }
        }

        private Visibility _loadingTextBlock;

        public Visibility loadingTextBlock
        {
            get { return _loadingTextBlock; }
            set
            {
                _loadingTextBlock = value;
                OnPropertyChanged(nameof(loadingTextBlock));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel(string currentDirectory)
        {
            //getImageSources(currentDirectory);
            getImageSourcesByChunk(currentDirectory);
        }

        public async void getImageSources(string currentDirectory)
        {
            imageCount = "";
            listBoxVisibility = Visibility.Collapsed;
            loadingTextBlock = Visibility.Visible;
            Images = await ImageLoader.LoadImagesAsync(currentDirectory);
            loadingTextBlock = Visibility.Collapsed;
            listBoxVisibility = Visibility.Visible;
            currentDir = currentDirectory;
            imageCount = $"Images Loaded: {Images?.Count ?? 0}";
        }

        public async void getImageSourcesByChunk(string currentDirectory)
        {
            imageCount = "";
            listBoxVisibility = Visibility.Collapsed;
            loadingTextBlock = Visibility.Visible;

            string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

            var directoryFiles = Directory.GetFiles(currentDirectory).Where(file => ImageExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase))).ToArray();

            List<ImageModel> localSource = new List<ImageModel>();
            Images.Clear();

            if (directoryFiles.Length < 9)
            {
                Images = await ImageLoader.LoadImagesAsync(currentDirectory);
                loadingTextBlock = Visibility.Collapsed;
                listBoxVisibility = Visibility.Visible;
                currentDir = currentDirectory;
                imageCount = $"Images Loaded: {Images?.Count ?? 0}";
                var listBox = Application.Current.MainWindow.FindName("ImgList") as ListBox;
                listBox.Focus();
                return;
            }

            for (var i = 0; i < directoryFiles.Length; i++)
            {
                var imgPath = directoryFiles[i];

                ImageModel imgModel = new()
                {
                    imgSource = await ImageLoader.CreateBitMapImageFromFile(imgPath),
                    uri = imgPath,
                    imageName = ImageLoader.GetFileNameNoExt(imgPath)
                };

                localSource.Add(imgModel);

                //we have done 10 iterations or have gotten to the end
                if (i % 9 == 0 || i == (directoryFiles.Length - 1))
                {
                    foreach (var source in localSource)
                    {
                        Images.Add(source);
                    }
                    imageCount = $"Images Loaded: {Images?.Count ?? 0}";
                    localSource.Clear();
                }
                if (i == 9)
                {
                    loadingTextBlock = Visibility.Collapsed;
                    listBoxVisibility = Visibility.Visible;
                    currentDir = currentDirectory;
                }
            }
            var listBox1 = Application.Current.MainWindow.FindName("ImgList") as ListBox;
            listBox1.Focus();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
