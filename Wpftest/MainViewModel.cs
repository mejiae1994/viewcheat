using System.Collections.ObjectModel;
using System.ComponentModel;

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

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel(string currentDirectory)
        {
            getImageSources(currentDirectory);
        }

        public async void getImageSources(string currentDirectory)
        {
            IsLoading = true;
            Images = await ImageLoader.LoadImagesAsync(currentDirectory);
            IsLoading = false;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
