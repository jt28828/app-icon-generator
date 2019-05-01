using System;
using System.IO;
using AppIconGenerator.Models;

namespace AppIconGenerator.Utilities
{
    public static class FileUtils
    {
        /// <summary> Allows the user to select a folder to save the file to </summary>
        public static string SelectFolder()
        {
            var folderPicker = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.MyPictures
            };

            var response = folderPicker.ShowDialog();
            if (response != null && response != false)
            {
                // Return the picked folder
                return folderPicker.SelectedPath;
            }

            // Cancelled by user
            return null;
        }

        public static SelectedFile SelectFile()
        {
            var filePicker = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();

            var response = filePicker.ShowDialog();
            if (response == null || response == false)
            {
                // Cancelled by user
                return null;
            }

            // Return the file
            Stream file = filePicker.OpenFile();

            return new SelectedFile
            {
                FileContents = file,
                FileName = filePicker.FileName
            };
        }

        public static string SaveToFile(byte[] bytes, string folderPath, string fileName)
        {
            var fileLocation = Path.Combine(folderPath, fileName);
            using (FileStream file = new FileStream(fileLocation, FileMode.OpenOrCreate))
            {
                // Write to the file
                using (BinaryWriter writer = new BinaryWriter(file))
                {
                    writer.Write(bytes);
                }
            }

            return fileLocation;
        }
    }
}