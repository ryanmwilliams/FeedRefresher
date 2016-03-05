using FeedRefresher.Feeds.Interfaces;
using System.Collections.Generic;
using FeedRefresher.Feeds.FRModels;
using FeedRefresher.Feeds.Utilities;
using FeedRefresher.Feeds.Podcasts.BaseRss;
using FRUpdater.Feeds.FRModels;

namespace FeedRefresher.Feeds.Services
{
    public class FeedRetrievalService : IFeedRetrievalService
    {
        public List<FRPodcastFileToProcess> GetPodcastFilesForProcessing(List<IRetrievablePodcast> podcasts)
        {
            var podcastFilesToProcess = new List<FRPodcastFileToProcess>();

            var podcastFileProcessor = new PodcastFileProcessor();

            foreach (var podcast in podcasts)
            {
                var processablePodcasts =
                    podcastFileProcessor.BuildPodcastFilesForPlaylists<RssRootBase>(podcast);

                if (HasNoPodcasts(processablePodcasts))
                    continue;

                podcastFilesToProcess.AddRange(processablePodcasts);
            }

            return podcastFilesToProcess;
        }

        private bool HasNoPodcasts(List<FRPodcastFileToProcess> processablePodcasts)
        {
            return processablePodcasts == null || processablePodcasts.Count == 0;
        }
    }
}
