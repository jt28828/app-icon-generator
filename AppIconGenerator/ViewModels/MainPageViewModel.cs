using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Xml;
using AppIconGenerator.Annotations;
using AppIconGenerator.Models;
using AppIconGenerator.Utilities;
using SkiaSharp;
using Svg;
using ExtendedSVG = SkiaSharp.Extended.Svg;

namespace AppIconGenerator.ViewModels
{
    /// <summary>
    /// Controls the Main page
    /// </summary>
    public class MainPageViewModel : INotifyPropertyChanged
    {
        // The File types support. Only SVG for now
        private static readonly string[] SupportedFileTypes = new string[] {"Vector File|*.svg"};

        private string _svgContent;

        private SelectedFile _selectedImage;

        public SelectedFile SelectedImage
        {
            get => _selectedImage;
            set
            {
                _selectedImage = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage _selectedImagePreview;

        public BitmapImage SelectedImagePreview
        {
            get => _selectedImagePreview;
            set
            {
                _selectedImagePreview = value;
                OnPropertyChanged();
            }
        }

        public MainPageViewModel()
        {
        }

        /// <summary>
        /// Opens the file picker for selecting files
        /// </summary>
        public async void OpenFilePicker()
        {
            SelectedFile file = FileUtils.SelectFile();

            if (file == null)
            {
                // User didn't pick anything
                return;
            }

            string fileType;
            try
            {
                string fileName = file.FileName;
                fileType =
                    $"{fileName[fileName.Length - 3]}{fileName[fileName.Length - 2]}{fileName[fileName.Length - 1]}";
            }
            catch (Exception)
            {
                // Filename doesn't exist
                MessageBox.Show("Invalid File", "Only SVG files are supported");
                return;
            }

            // Make sure file is an SVG
            if (fileType.ToLower().Contains("svg"))
            {
                // Save to Storage
                SelectedImage = file;

                _svgContent = await ReadStreamAsTextAsync(file.FileContents);
                SelectedImagePreview = ToBitmapImage(await SvgStringToBitmap(_svgContent));
            }
            else
            {
                // File isn't an svg
                System.Windows.MessageBox.Show("Invalid File", "Only SVG files are supported");
            }
        }

        private static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        public async void SaveImages(string name = "icon")
        {
            // Let the user pick the folder to save the file(s) in
            var folder = FileUtils.SelectFolder();

            // Save Android and iOS and alert user
            Task androidTask = SaveAndroid(folder, name);
            Task iOsTask = SaveIos(folder, name);
            Task webTask = SaveWeb(folder);

            MessageBox.Show(
                $"Icons are now generating in the background. This will take a while, please be patient and wait for the completion dialog",
                "Warning", MessageBoxButton.OK);

            // Alert the User when both are complete
            await Task.WhenAll(androidTask, iOsTask, webTask);
            MessageBox.Show($"Images were saved at: {folder}", "Success");
        }

        /// <summary>
        /// Converts an SVG File into an SVG object
        /// </summary>
        /// <param name="file">The File to convert</param>
        private static ExtendedSVG.SKSvg SvgFromFile(SelectedFile file)
        {
            var stream = file.FileContents;
            var svg = new ExtendedSVG.SKSvg(new SKSize(200, 200));
            svg.Load(stream);

            return svg;
        }

        private async Task<Bitmap> SvgStringToBitmap(string svgText, int size = 1000)
        {
            XmlDocument doc = new XmlDocument {InnerXml = svgText};
            var svg = SvgDocument.Open(doc);

            // Resize the image, keeping its original dimensions
            SizeF newDimensions = new SizeF(svg.Width.Value, svg.Height.Value);
            svg.RasterizeDimensions(ref newDimensions, size, 0);

            // Set new dimensions onto image
            svg.Width = newDimensions.Width;
            svg.Height = newDimensions.Height;

            Bitmap bitmap = svg.Draw();
            // Wait a little bit for the image to render properly otherwise it corrupts. 500ms seems to work well
            await Task.Delay(500);
            return bitmap;
        }

        private static async Task<string> ReadStreamAsTextAsync(Stream stream)
        {
            string fileContents;
            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                fileContents = await reader.ReadToEndAsync();
            }

            return fileContents;
        }

        private async Task SaveAndroid(string baseFolder, string fileName)
        {
            // Create Android Subdirectories
            var androidDir = Path.Combine(baseFolder, "Android/");

            if (!Directory.Exists(androidDir))
            {
                Directory.CreateDirectory(androidDir);
            }

            var taskList = new List<Task>();

            foreach (KeyValuePair<string, int> fileSize in AndroidFileSizes.All)
            {
                // Put all image generation onto a separate thread
                var newTask = Task.Run(async () =>
                {
                    // Throw each onto seperate threads
                    var thisFolder = Path.Combine(androidDir, $"{fileSize.Key}/");
                    if (!Directory.Exists(thisFolder))
                    {
                        Directory.CreateDirectory(thisFolder);
                    }

                    var bitmap = await SvgStringToBitmap(_svgContent, fileSize.Value);
                    WriteImageToFile(bitmap, thisFolder, fileName);
                });

                // Add to list to keep track
                taskList.Add(newTask);
            }

            // Wait for all images to be completed before completing the function
            await Task.WhenAll(taskList);
        }

        private async Task SaveIos(string baseFolder, string fileName)
        {
            // First create the directories if required
            (string iPhoneDir, string iPadDir, string imageDir) = CreateiOSDirectories(baseFolder);

            var taskList = new List<Task>();

            // Then create the app-icon sizes
            foreach (KeyValuePair<string, int> fileSize in iOSFileSizes.iPhoneSizes)
            {
                // Throw each onto separate threads
                var thisImageTask = Task.Run(async () =>
                {
                    var bitmap = await SvgStringToBitmap(_svgContent, fileSize.Value);
                    WriteImageToFile(bitmap, iPhoneDir, $"{fileName}-{fileSize.Key}");
                });
                taskList.Add(thisImageTask);
            }

            foreach (KeyValuePair<string, int> fileSize in iOSFileSizes.iPadSizes)
            {
                // Throw each onto separate threads
                var thisImageTask = Task.Run(async () =>
                {
                    var bitmap = await SvgStringToBitmap(_svgContent, fileSize.Value);
                    WriteImageToFile(bitmap, iPadDir, $"{fileName}-{fileSize.Key}");
                });
                taskList.Add(thisImageTask);
            }

            // Finally create the 1x,2x,3x images

            foreach (KeyValuePair<string, int> fileSize in iOSFileSizes.ImageSizes)
            {
                // Throw each onto separate threads
                var thisImageTask = Task.Run(async () =>
                {
                    var bitmap = await SvgStringToBitmap(_svgContent, fileSize.Value);
                    WriteImageToFile(bitmap, imageDir, $"{fileName}{fileSize.Key}");
                });
                taskList.Add(thisImageTask);
            }

            // Wait for all images to be completed before completing the function
            await Task.WhenAll(taskList);
        }

        /// <summary>
        /// Saves icons in PWA format to the Web folder
        /// </summary>
        private async Task SaveWeb(string baseFolder)
        {
            // Create Android Subdirectories
            var webDir = Path.Combine(baseFolder, "Web/");

            if (!Directory.Exists(webDir))
            {
                Directory.CreateDirectory(webDir);
            }

            var taskList = new List<Task>();

            foreach (KeyValuePair<string, int> fileSize in WebFileSizes.All)
            {
                // Put all image generation onto a separate thread
                var newTask = Task.Run(async () =>
                {
                    // Throw each onto separate threads
                    var bitmap = await SvgStringToBitmap(_svgContent, fileSize.Value);
                    WriteImageToFile(bitmap, webDir, fileSize.Key);
                });

                // Add to list to keep track
                taskList.Add(newTask);
            }

            // Wait for all images to be completed before completing the function
            await Task.WhenAll(taskList);
        }

        private Tuple<string, string, string> CreateiOSDirectories(string baseFolder)
        {
            // Create iOS Subdirectory
            var iOSDir = Path.Combine(baseFolder, "iOS/");

            if (!Directory.Exists(iOSDir))
            {
                Directory.CreateDirectory(iOSDir);
            }

            // Also create directory for both iPhone and iPad underneath the icon folder
            var iconDir = Path.Combine(iOSDir, "Icons/");
            if (!Directory.Exists(iconDir))
            {
                Directory.CreateDirectory(iconDir);
            }

            var iPhoneDir = Path.Combine(iconDir, "iPhone/");
            var iPadDir = Path.Combine(iconDir, "iPad/");

            if (!Directory.Exists(iPhoneDir))
            {
                Directory.CreateDirectory(iPhoneDir);
            }

            if (!Directory.Exists(iPadDir))
            {
                Directory.CreateDirectory(iPadDir);
            }

            // Then create a directory for the regular image resizes if using as an image
            var imageDir = Path.Combine(iOSDir, "Images/");
            if (!Directory.Exists(imageDir))
            {
                Directory.CreateDirectory(imageDir);
            }

            return Tuple.Create(iPhoneDir, iPadDir, imageDir);
        }

        private void WriteImageToFile(Bitmap image, string folder, string fileName)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Png);
                // Save the PNG in memory
                var png = memoryStream.ToArray();
                // Create a file name
                var fullFilename = $"{fileName}.png";
                // Finally save the file
                FileUtils.SaveToFile(png, folder, fullFilename);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}