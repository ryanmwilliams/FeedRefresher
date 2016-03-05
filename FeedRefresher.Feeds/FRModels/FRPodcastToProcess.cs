using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedRefresher.Feeds.FRModels
{
    public class FRPodcastToProcess
    {
        public string DestinationPathForMp3 { get; set; }

        public string PathToDownloadedMp3 { get; set; }

        public List<string> PlaylistPathsToIncludeIn { get; set; }

        public DateTime ReleaseDateOfPodcastFile { get; set; }
    }
}
