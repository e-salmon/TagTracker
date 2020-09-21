using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTracker.TagLibService.Interfaces;
using TagTracker.TagLibService.Objects;
using TagTracker.Utility;

namespace TagTracker.TagLibService.Services
{
    public static class TagReaderService
    {
        private const string DefaultExtensions = "*.mp3,*.wma";

        /// <summary>
        /// Reads the track info and initializes a <see cref="TrackInfo"/> object.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>A <see cref="TrackInfo"/> object.</returns>
        public static ITrackInfo ReadTrackInfo(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                return new TrackInfo(filePath);
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        /// <summary>
        /// Reads the track information in all media files found in the folder specified
        /// by <paramref name="path"/>, including subfolders if specified.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="includeSubfolders">If <c>true</c>, include subfolders.</param>
        /// <returns>
        /// A list of <see cref="ITrackInfo"/>.
        /// </returns>
        public static IEnumerable<ITrackInfo> ReadTracksInfo(string path, bool includeSubfolders)
        {
            return ReadTracksInfo(path, includeSubfolders, DefaultExtensions);
        }

        /// <summary>
        /// Reads the track information in all media files found in the folder specified
        /// by <paramref name="path"/>, including subfolders if specified.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="includeSubfolders">If <c>true</c>, include subfolders.</param>
        /// <param name="extensions">The extensions.</param>
        /// <returns>
        /// A list of <see cref="ITrackInfo"/>.
        /// </returns>
        public static IEnumerable<ITrackInfo> ReadTracksInfo(
            string path,
            bool includeSubfolders,
            string extensions)
        {
            var results = new List<ITrackInfo>();

            if (includeSubfolders)
            {
                IEnumerable<string> folders = DirectoryService.SafeFolders(path);
                if (folders == null)
                {
                    return results;
                }

                foreach (string folderName in folders)
                {
                    results.AddRange(ReadTracksInfo(folderName, true, extensions));
                }
            }

            return results
                .Concat(from ext in extensions.Split(',')
                        from file in Directory.EnumerateFiles(path, ext)
                        select ReadTrackInfo(file))
                .OrderBy(x => x.FileName);
        }
    }
}
