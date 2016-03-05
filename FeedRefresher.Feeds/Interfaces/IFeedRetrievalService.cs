using FeedRefresher.Feeds.FRModels;
using System.Collections.Generic;

namespace FeedRefresher.Feeds.Interfaces
{
    using FRUpdater.Feeds.FRModels;

    public interface IFeedRetrievalService
    {
        List<FRPodcastFileToProcess> GetPodcastFilesForProcessing(List<IRetrievablePodcast> podcasts);
    }
}