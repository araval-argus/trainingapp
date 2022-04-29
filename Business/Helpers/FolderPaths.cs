using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Business.Helpers
{
    public class FolderPaths
    {
        public static string PathToProfileFolder = Path.Combine("Resources", "Assets", "ProfilePic");
        public static string PathToImageDumpFolder = Path.Combine("Resources", "Assets", "ImagesDump");
    }
}
