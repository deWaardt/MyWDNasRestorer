using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WD_File_Recovery
{
    internal static class Session
    {
        public static string? dbFilePath;
        public static string? filesPath;
        public static string? outputPath;
        public static List<Item> allItems;
        public static string[] wdddirFiles;

        public static int totalCount;
        public static int current;

        public static string currentFile;
        public static string filesProcessed;

        public static bool ready = false;
    }
}
