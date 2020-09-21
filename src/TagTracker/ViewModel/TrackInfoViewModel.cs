using System.ComponentModel;
using System.IO;
using TagTracker.TagLibService.Interfaces;

namespace TagTracker.ViewModel
{
    public class TrackInfoViewModel : INotifyPropertyChanged
    {
        public ITrackInfo TrackInfo { get; set; }

        public bool HasTrackNumber
        {
            get { return TrackInfo.TrackNumber != 0; }
        }

        public bool HasTrackTitle
        {
            get { return !string.IsNullOrEmpty(TrackInfo.TrackTitle); }
        }

        public bool HasTrackArtist
        {
            get { return !string.IsNullOrEmpty(TrackInfo.TrackArtist); }
        }

        public bool HasAlbumTitle
        {
            get { return !string.IsNullOrEmpty(TrackInfo.AlbumTitle); }
        }

        public bool HasAlbumArtist
        {
            get { return !string.IsNullOrEmpty(TrackInfo.AlbumArtist); }
        }

        public bool HasAlbumYear
        {
            get { return TrackInfo.AlbumYear != 0; }
        }

        public bool HasAlbumArt
        {
            get { return TrackInfo.AlbumArtCount != 0; }
        }

        public int AlbumArtCount
        {
            get { return TrackInfo.AlbumArtCount; }
        }

        public bool HasDecentBitrate
        {
            get { return TrackInfo.DecentBitrate; }
        }

        public string TrackType
        {
            get
            {
                return string.IsNullOrEmpty(TrackInfo.FileName)
                    ? string.Empty
                    : Path.GetExtension(TrackInfo.FileName);
            }
        }

        public void Refresh()
        {
            TrackInfo.Refresh();

            // Ensure all bindings are made aware.
            OnPropertyChanged("HasTrackNumber");
            OnPropertyChanged("HasTrackTitle");
            OnPropertyChanged("HasTrackArtist");
            OnPropertyChanged("HasAlbumTitle");
            OnPropertyChanged("HasAlbumArtist");
            OnPropertyChanged("HasAlbumYear");
            OnPropertyChanged("HasAlbumArt");
            OnPropertyChanged("HasDecentBitrate");
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
