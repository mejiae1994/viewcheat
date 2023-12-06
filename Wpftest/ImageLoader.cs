using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Wpftest
{

    public static class ImageLoader
    {
        private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        public static async Task<ObservableCollection<ImageModel>> LoadImagesAsync(string currentDirectory)
        {
            if (Directory.Exists(currentDirectory))
            {
                try
                {
                    var directoryFiles = Directory.GetFiles(currentDirectory).Where(file => ImageExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)));

                    return await Task.Run(() => LoadImagesFromPaths(directoryFiles));
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"IO exception: {ex.Message}");
                }
            }
            return new ObservableCollection<ImageModel>();
        }

        private static async Task<ObservableCollection<ImageModel>> LoadImagesFromPaths(IEnumerable<string> imgPaths)
        {
            ObservableCollection<ImageModel> sourceCollection = new ObservableCollection<ImageModel>();
            Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            foreach (var imgPath in imgPaths)
            {
                using (FileStream SourceStream = new FileStream(imgPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, FileOptions.Asynchronous))
                {
                    ImageModel imgModel = new()
                    {
                        imgSource = await CreateBitMapImageFromFile(imgPath),
                        uri = imgPath,
                        imageName = GetFileNameNoExt(imgPath)
                    };
                    sourceCollection.Add(imgModel);
                }
            }
            watch.Stop();
            System.Diagnostics.Debug.WriteLine(watch.ElapsedMilliseconds + " ms.");
            return sourceCollection;
        }

        private static async Task<BitmapImage> CreateBitMapImageFromFile(string imgPath)
        {
            return await Task.Run(() =>
            {
                BitmapImage bitMapImg = new BitmapImage();
                using (FileStream SourceStream = new FileStream(imgPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, FileOptions.Asynchronous))
                {
                    bitMapImg.BeginInit();
                    bitMapImg.CacheOption = BitmapCacheOption.OnLoad;
                    bitMapImg.DecodePixelHeight = 400;
                    bitMapImg.DecodePixelWidth = 400;
                    bitMapImg.StreamSource = SourceStream;
                    bitMapImg.EndInit();
                }
                bitMapImg.Freeze();
                return bitMapImg;
            });
        }

        private static string GetFileNameNoExt(string uri)
        {
            string[] uriParts = uri.Split('\\');
            return Path.GetFileNameWithoutExtension(uriParts[uriParts.Length - 1]);
        }
    }
}
