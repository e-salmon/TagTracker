namespace TagTracker.TagLibService.Interfaces
{
    public interface ITrackInfo
    {
        string FileName { get; set; }

        int TrackNumber { get; set; }

        string TrackTitle { get; set; }

        string TrackArtist { get; set; }

        string AlbumTitle { get; set; }

        string AlbumArtist { get; set; }

        int AlbumYear { get; set; }

        int AlbumArtCount { get; set; }

        int Bitrate { get; set; }

        bool VariableBitrate { get; set; }

        string Duration { get; set; }

        string Comment { get; set; }

        int DiscNumber { get; set; }

        bool DecentBitrate { get; }

        byte[] AlbumArt(int index);

        void Refresh();
    }
}
