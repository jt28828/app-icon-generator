namespace AppIconGenerator.Models
{
    /// <summary>
    /// Contains all of the file sizes for iOS Icons.
    /// </summary>
    class iOSFileSizes
    {
        /// <summary> Medium Density </summary>
        public const int MDPI = 48;
        /// <summary> High Density </summary>
        public const int HDPI = 72;
        /// <summary> Extra High Density </summary>
        public const int XHDPI = 96;
        /// <summary> Extra Extra High Density </summary>
        public const int XXHDPI = 144;
        /// <summary> Extra Extra Extra High Density </summary>
        public const int XXXHDPI = 192;
        /// <summary> Application Icon for the Play store </summary>
        public const int PlayStore = 512;
        /// <summary> Contains all of the recommended icon sizes for Android. Current as of 2018-07-18 </summary>
        public static int[] All = new int[] { MDPI, HDPI, XHDPI, XXHDPI, XXXHDPI, PlayStore };
        /// <summary> Contains all of the recommended icon size names Android. Current as of 2018-07-18 </summary>
        public static string[] AllNames = new string[] { "MDPI", "HDPI", "XHDPI", "XXHDPI", "XXXHDPI", "PlayStore" };

    }
}
