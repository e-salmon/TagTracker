using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using TagLib.Mpeg;
using TagTracker.TagLibService.Interfaces;
using IOFile = System.IO.File;
using TagLibFile = TagLib.File;

namespace TagTracker.TagLibService.Objects
{
    public class TrackInfo : ITrackInfo, INotifyPropertyChanged
    {
        public TrackInfo(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("The file path is null or empty.", nameof(filePath));
            }

            if (!IOFile.Exists(filePath))
            {
                throw new FileNotFoundException();
            }

            FileName = filePath;
            LoadTrackInfo();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string FileName { get; set; }

        public int TrackNumber { get; set; }

        public string TrackTitle { get; set; }

        public string TrackArtist { get; set; }

        public string AlbumTitle { get; set; }

        public string AlbumArtist { get; set; }

        public int AlbumYear { get; set; }

        public int AlbumArtCount { get; set; }

        public int Bitrate { get; set; }

        public bool VariableBitrate { get; set; }

        public int DiscNumber { get; set; }

        public string Duration { get; set; }

        public string Comment { get; set; }

        public bool DecentBitrate => Bitrate >= 192 || (VariableBitrate && Bitrate >= 170);

        public byte[] AlbumArt(int index)
        {
            if (index < 0 || index >= AlbumArtCount || !IOFile.Exists(FileName))
            {
                return null;
            }

            try
            {
                using (var mediaFile = TagLibFile.Create(FileName))
                {
                    return mediaFile.Tag.Pictures[index].Data.Data;
                }
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        public void Refresh()
        {
            LoadTrackInfo();

            // Ensure all bindings are made aware.
            OnPropertyChanged("TrackNumber");
            OnPropertyChanged("TrackTitle");
            OnPropertyChanged("TrackArtist");
            OnPropertyChanged("AlbumTitle");
            OnPropertyChanged("AlbumArtist");
            OnPropertyChanged("AlbumYear");
            OnPropertyChanged("AlbumArtCount");
            OnPropertyChanged("Bitrate");
            OnPropertyChanged("VariableBitrate");
            OnPropertyChanged("DiscNumber");
            OnPropertyChanged("Duration");
            OnPropertyChanged("Comment");
        }

        private void LoadTrackInfo()
        {
            try
            {
                using (var mediaFile = TagLibFile.Create(FileName))
                {

                    TrackNumber = (int)mediaFile.Tag.Track;
                    TrackTitle = mediaFile.Tag.Title;
                    TrackArtist = mediaFile.Tag.JoinedPerformers;
                    AlbumTitle = mediaFile.Tag.Album;
                    AlbumArtist = mediaFile.Tag.JoinedAlbumArtists;
                    AlbumYear = (int)mediaFile.Tag.Year;
                    AlbumArtCount = mediaFile.Tag.Pictures.Length;
                    DiscNumber = (int)mediaFile.Tag.Disc == 0 ? 1 : (int)mediaFile.Tag.Disc;
                    Duration = mediaFile.Properties.Duration.ToString(@"mm\:ss", CultureInfo.InvariantCulture);
                    Comment = mediaFile.Tag.Comment;

                    // Seems to need to go through the codecs to have the Bitrate read 
                    // from AudioBitrate properly... Hmmm...
                    foreach (AudioHeader header in mediaFile.Properties.Codecs.Cast<AudioHeader>())
                    {
                        VariableBitrate = header.VBRIHeader.Present || header.XingHeader.Present;
                    }

                    Bitrate = mediaFile.Properties.AudioBitrate;
                }
            }
            catch
            {
                // Do nothing for now, will try and think of something to do inthis case...
            }
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
