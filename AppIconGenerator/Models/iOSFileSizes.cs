using System.Collections.Generic;

namespace AppIconGenerator.Models
{
    /// <summary>
    /// Contains all of the file sizes for iOS Icons.
    /// </summary>
    internal static class iOSFileSizes
    {
        #region iPhone

        // Notifications
        private const int iphone_notification_2x = 40;
        private const int iphone_notification_3x = 60;

        // Settings
        private const int iphone_settings_2x = 58;

        private const int iphone_settings_3x = 87;

        // Spotlight
        private const int iphone_spotlight_2x = 80;
        private const int iphone_spotlight_3x = 120;

        // App Icon
        private const int iphone_icon_2x = 120;
        private const int iphone_icon_3x = 180;

        #endregion

        #region iPad

        // Notification
        private const int ipad_notification_1x = 20;
        private const int ipad_notification_2x = 40;

        // Settings
        private const int ipad_settings_1x = 29;
        private const int ipad_settings_2x = 58;

        // Spotlight
        private const int ipad_spotlight_1x = 40;
        private const int ipad_spotlight_2x = 80;

        // App Icon
        private const int ipad_icon_1x = 76;
        private const int ipad_icon_2x = 152;
        private const int ipad_pro_icon_2x = 167;
        private const int app_store = 1024;

        #endregion

        #region Regular Images

        // Image Sizes
        private const int Image_1x = 100;
        private const int Image_2x = 200;
        private const int Image_3x = 300;

        #endregion

        /// <summary> Contains all of the recommended icon sizes for iPhone. Current as of 2018-17-06 </summary>
        public static readonly Dictionary<string, int> iPhoneSizes = new Dictionary<string, int>
        {
            {"iphone_notification_2x", iphone_notification_2x},
            {"iphone_notification_3x", iphone_notification_3x},
            {"iphone_settings_2x", iphone_settings_2x},
            {"iphone_settings_3x", iphone_settings_3x},
            {"iphone_spotlight_2x", iphone_spotlight_2x},
            {"iphone_spotlight_3x", iphone_spotlight_3x},
            {"iphone_icon_2x", iphone_icon_2x},
            {"iphone_icon_3x", iphone_icon_3x},
        };

        public static readonly Dictionary<string, int> iPadSizes = new Dictionary<string, int>
        {
            {"ipad_notification_1x", ipad_notification_1x},
            {"ipad_notification_2x", ipad_notification_2x},
            {"ipad_settings_1x", ipad_settings_1x},
            {"ipad_settings_2x", ipad_settings_2x},
            {"ipad_spotlight_1x", ipad_spotlight_1x},
            {"ipad_spotlight_2x", ipad_spotlight_2x},
            {"ipad_icon_1x", ipad_icon_1x},
            {"ipad_icon_2x", ipad_icon_2x},
            {"ipad_pro_icon_2x", ipad_pro_icon_2x},
            {"app_store", app_store}
        };

        public static readonly Dictionary<string, int> ImageSizes = new Dictionary<string, int>
        {
            {"1x", Image_1x},
            {"2x", Image_2x},
            {"3x", Image_3x},
        };
    }
}