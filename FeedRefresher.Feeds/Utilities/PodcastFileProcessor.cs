using FeedRefresher.Core.Logging;
using FeedRefresher.Core.Web;
using FeedRefresher.Feeds.FRModels;
using FeedRefresher.Feeds.Podcasts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using FeedRefresher.Feeds.Podcasts.BaseRss;
using FeedRefresher.Feeds.Interfaces;
using System.Text;
using FeedRefresher.Core.Constants;
using FRUpdater.Feeds.FRModels;

namespace FeedRefresher.Feeds.Utilities
{
    public class PodcastFileProcessor
    {
        public List<FRPodcastFileToProcess> BuildPodcastFilesForPlaylists<T>(IRetrievablePodcast podcast) where T : IRss
        {
            return BuildPodcastFilesForPlaylists<RssRootBase>(
                    podcast.PodcastUrl,
                    podcast.FeedId,
                    podcast.DestinationDirectory,
                    podcast.TargetPlaylistPaths,
                    podcast.MaxFilesToDownload,
                    podcast.FilterTitlesOn);
        }

        private List<FRPodcastFileToProcess> BuildPodcastFilesForPlaylists<T>(
            Uri PodcastUri,
            int feedId,
            string destinationDirectoryOfAllPodcastFiles,            
            List<string> playlistPathsToIncludeIn,
            int maxNewToDownload = IntConstants.MaxNewToDownload,
            List<string> filterOnTitles = null) where T : IRss
        {
            Log.Debug("-----------------------------------------------------");
            Log.Debug("Starting processing of podcasts for feed id: '{0}'...", feedId);

            var rssFeed = DownloadClient.DownloadUrlContentIntoModel<T>(PodcastUri);

            if (FeedIsEmpty(rssFeed))
            {
                Log.Debug("Feed is empty, skipping.");
                return new List<FRPodcastFileToProcess>();
            }

            var podcastItems = rssFeed.Channel.Item;

            Log.Debug("A total of: '{0}' podcasts were found on this feed.", podcastItems.Count());
            Log.Debug("A maximum of: '{0}' files will be downloaded from: '{1}'.", maxNewToDownload, PodcastUri);

            var podcastFilesToProcess = new List<FRPodcastFileToProcess>();

            var filteredPodcats = FilterPodcasts(podcastItems, filterOnTitles);

            var podcastsToProcess = filteredPodcats.OrderByDescending(x => Convert.ToDateTime(x.PubDate))
                                                   .Take(maxNewToDownload);

            Log.Debug("A total of: '{0}' podcasts will be checked.", podcastsToProcess.Count());

            foreach (var podcastFile in podcastsToProcess)
            {
                var podcastFileUrl = new Uri(podcastFile.Enclosure.Url);
                var releaseDateOfPodcast = Convert.ToDateTime(podcastFile.PubDate);

                if (PathHelper.PodcastExists(
                                        podcastFileUrl,
                                        releaseDateOfPodcast,
                                        feedId,
                                        destinationDirectoryOfAllPodcastFiles))
                {
                    Log.Debug("Podcast at: '{0}' already exists in the destination.", podcastFileUrl);
                    continue;
                }

                var podcastFileToProcess = new FRPodcastFileToProcess()
                {
                    ReleaseDateOfPodcastFile = releaseDateOfPodcast,
                    PlaylistPathsToIncludeIn = playlistPathsToIncludeIn
                };

                var downloadedFilePath = PathHelper.DownloadFilePath(podcastFileUrl, releaseDateOfPodcast, feedId);

                if (!DownloadClient.DownloadFile(podcastFileUrl, downloadedFilePath))
                {
                    Log.Debug("Skipping file at: '{0}' which could not be downloaded.", podcastFileUrl);
                    continue;
                }

                podcastFileToProcess.PathToDownloadedMp3 = PathHelper.SetPathToMp3(downloadedFilePath);
                podcastFileToProcess.DestinationPathForMp3 = PathHelper.SetDestinationPathForMp3(
                                                                            podcastFileToProcess.PathToDownloadedMp3,
                                                                            destinationDirectoryOfAllPodcastFiles);

                podcastFilesToProcess.Add(podcastFileToProcess);
            }

            Log.Debug("Completed processing of podcasts for feed id: '{0}'.", feedId);

            return podcastFilesToProcess;
        }

        private bool FeedIsEmpty<T>(T rssFeed) where T : IRss
        {
            return rssFeed == null || 
                   rssFeed.Channel == null || 
                   rssFeed.Channel.Item == null;
        }

        private List<Item> FilterPodcasts(List<Item> podcasts, List<string> filterOnTitles)
        {
            if (IsMissingFilters(filterOnTitles))
            {
                Log.Debug("There are no filters for this podcast.");
                return podcasts;
            }

            Log.Debug("Filtering podcasts...");

            var filters = new StringBuilder();

            foreach(var title in filterOnTitles)
            {
                filters.AppendFormat("{0}|", title);
            }

            Log.Debug("Filtering on: '{0}'", filters.ToString());


            var result = from p in podcasts
                         where filterOnTitles.Any(val => p.Title.ToLower()
                                                                .Contains(val.ToLower()))
                         select p;

            return result.ToList();
        }

        private bool IsMissingFilters(List<string> filterOnTitles)
        {
            return filterOnTitles == null || filterOnTitles.Count() <= 0;
        }
    }
}
