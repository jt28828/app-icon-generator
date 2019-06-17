using System.Collections.Generic;

namespace AppIconGenerator.Models
{
    public class WebFileSizes
    {
        private const int x1 = 48;
        private const int x2 = 72;
        private const int x3 = 96;
        private const int x4 = 120;
        private const int x5 = 144;
        private const int x6 = 168;
        private const int x7 = 180;
        private const int x8 = 192;
        private const int x9 = 256;
        private const int x10 = 512;

        public static readonly Dictionary<string, int> All = new Dictionary<string, int>
        {
            {"icon-48x48", x1},
            {"icon-72x72", x2},
            {"icon-96x96", x3},
            {"icon-120x120", x4},
            {"icon-144x144", x5},
            {"icon-168x168", x6},
            {"icon-180x180", x7},
            {"icon-192x192", x8},
            {"icon-256x256", x9},
            {"icon-512x512", x10},
        };
    }
}