using System.IO;

namespace AppIconGenerator.Models
{
    public class SelectedFile
    {
        public Stream FileContents { get; set; }
        public string FileName { get; set; }
    }
}