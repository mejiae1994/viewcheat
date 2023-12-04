using System.Collections.ObjectModel;

namespace Wpftest
{
    public class MainViewModel
    {

        public ObservableCollection<ImageModel> Images { get; set; }

        public MainViewModel()
        {

        }

        private async void getImageSources()
        {

            Images = await ImageLoader.LoadImagesAsync();
        }
    }
}
