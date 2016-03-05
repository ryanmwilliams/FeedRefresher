using System;

namespace FeedRefresher.Playlists.Models
{
    public class DatedPlaylistFile : PlaylistFile
    {
        public DateTime PublishDate { get; set; }
    }
}
