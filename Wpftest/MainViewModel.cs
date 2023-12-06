using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Wpftest
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<ImageModel> _images;

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
            getImageSources(currentDirectory);
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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
