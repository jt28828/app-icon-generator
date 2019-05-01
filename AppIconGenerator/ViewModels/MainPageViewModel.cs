using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
                fileType = file.FileName[(file.FileName.Length - 3)].ToString() +
                           file.FileName[(file.FileName.Length - 2)].ToString() +
                           file.FileName[(file.FileName.Length - 1)].ToString();
            }
            catch (Exception)
            {
                // Filename doesn't exist
                System.Windows.MessageBox.Show("Invalid File", "Only SVG files are supported");
                return;
            }

            // Make sure file is an SVG
            if (fileType.ToLower().Contains("svg"))
            {
                // Save to Storage
                SelectedImage = file;

                _svgContent = await ReadStreamAsTextAsync(file.FileContents);
                SelectedImagePreview = ToBitmapImage(SvgStringToBitmap(_svgContent));
            }
            else
            {
                // File isn't an svg
                System.Windows.MessageBox.Show("Invalid File", "Only SVG files are supported");
            }
        }

        public static BitmapImage ToBitmapImage(Bitmap bitmap)
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

        public void SaveImages(string name = "icon")
        {
            // Let the user pick the folder to save the file(s) in
            var folder = FileUtils.SelectFolder();

            // Save Android then iOS
            SaveAndroid(folder, name);
            SaveIos(folder, name);

            // Alert the User
            System.Windows.MessageBox.Show($"Images were saved at: {folder}", "Success");
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

        private Bitmap SvgStringToBitmap(string svgText, int size = 1000)
        {
            XmlDocument doc = new XmlDocument {InnerXml = svgText};
            var svg = SvgDocument.Open(doc);

            SizeF ignore = new SizeF();
            svg.RasterizeDimensions(ref ignore, size, size);

            return svg.Draw();
        }

        private static async Task<string> ReadStreamAsTextAsync(Stream stream)
        {
            string fileContents = "";
            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                fileContents = await reader.ReadToEndAsync();
            }

            return fileContents;
        }

        private void SaveAndroid(string baseFolder, string fileName)
        {
            // Create Android Subdirectories
            var androidDir = Path.Combine(baseFolder, "Android/");

            if (!Directory.Exists(androidDir))
            {
                Directory.CreateDirectory(androidDir);
            }

            for (var i = 0; i < AndroidFileSizes.All.Length; i++)
            {
                var filesize = AndroidFileSizes.All[i];
                var imageSizeName = AndroidFileSizes.AllNames[i];
                // Throw each onto seperate threads
                Task.Run(async () =>
                {
                    var thisFolder = Path.Combine(androidDir, $"{imageSizeName}/");
                    if (!Directory.Exists(thisFolder))
                    {
                        Directory.CreateDirectory(thisFolder);
                    }

                    var bitmap = SvgStringToBitmap(_svgContent, filesize);
                    // Wait a little bit for the image to render properly otherwise it corrupts. 500ms seems to work well
                    await Task.Delay(500);
                    WriteImageToFile(bitmap, thisFolder, fileName);
                });
            }
        }

        private void SaveIos(string baseFolder, string fileName)
        {
            // Create iOS Subdirectory
            var iOSDir = Path.Combine(baseFolder, "iOS/");

            if (!Directory.Exists(iOSDir))
            {
                Directory.CreateDirectory(iOSDir);
            }

            for (var i = 0; i < iOSFileSizes.All.Length; i++)
            {
                var filesize = iOSFileSizes.All[i];
                var imageSizeName = iOSFileSizes.AllNames[i];

                // Throw each onto seperate threads
                Task.Run(async () =>
                {
                    var thisFolder = Path.Combine(iOSDir, $"{imageSizeName}/");
                    if (!Directory.Exists(thisFolder))
                    {
                        Directory.CreateDirectory(thisFolder);
                    }

                    var bitmap = SvgStringToBitmap(_svgContent, filesize);
                    // Wait a little bit for the image to render properly otherwise it corrupts. 500ms seems to work well
                    await Task.Delay(500);
                    WriteImageToFile(bitmap, thisFolder, fileName);
                });
            }
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