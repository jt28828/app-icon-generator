using System.Collections.Generic;

namespace AppIconGenerator.Models
{
    /// <summary>
    /// Contains all of the file sizes for Android Icons
    /// </summary>
    public static class AndroidFileSizes
    {
        /// <summary> Medium Density </summary>
        private const int MDPI = 48;

        /// <summary> High Density </summary>
        private const int HDPI = 72;

        /// <summary> Extra High Density </summary>
        private const int XHDPI = 96;

        /// <summary> Extra Extra High Density </summary>
        private const int XXHDPI = 144;

        /// <summary> Extra Extra Extra High Density </summary>
        private const int XXXHDPI = 192;

        /// <summary> Application Icon for the Play store </summary>
        private const int PlayStore = 1024;

        /// <summary> Contains all of the recommended icon sizes for Android. Current as of 2018-07-18 </summary>
        public static readonly Dictionary<string, int> All = new Dictionary<string, int>
        {
            {"MDPI", MDPI},
            {"HDPI", HDPI},
            {"XHDPI", XHDPI},
            {"XXHDPI", XXHDPI},
            {"XXXHDPI", XXXHDPI},
            {"PlayStore", PlayStore}
        };
    }
}