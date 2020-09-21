using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TagTracker.Utility
{
    public static class DirectoryService
    {
        private  const FileAttributes UnwantedMask = FileAttributes.Hidden | FileAttributes.System;

        public static IEnumerable<string> DriveNames()
        {
            return from drive in DriveInfo.GetDrives()
                where drive.IsReady
                select drive.RootDirectory.Name;
        }

        public static IEnumerable<string> SafeFolders(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
                return null;

            try
            {
                return from dir in dirInfo.EnumerateDirectories()
                    where (dir.Attributes & UnwantedMask) == 0
                    select dir.FullName;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }
    }
}
