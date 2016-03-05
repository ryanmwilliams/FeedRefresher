using System.Collections.Generic;

namespace FeedRefresher.Feeds.Interfaces
{
    public interface IFeedValidationService
    {
        bool AreValidFeeds(List<IRetrievablePodcast> podcast);
    }
}
