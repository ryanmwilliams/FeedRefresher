using FeedRefresher.Core.Constants;
using FeedRefresher.Core.FileSystem;
using FeedRefresher.Core.Logging;
using FeedRefresher.Feeds.FRModels;
using FeedRefresher.Feeds.Interfaces;
using FeedRefresher.Feeds.Services;
using FeedRefresher.Playlists;
using FeedRefresher.Playlists.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FeedRefresher.Main
{
    class Program
    {
        const string AppVersion = "(v.0.2.6)";

        static void Main(string[] args)
        {
            InitializeLogger();

            CreateAllRequiredDirectories();

            var podcastFeeds = GetPodcastFeeds();
            
            var downloadedPodcastFilesToProcess = GetDownloadedFilesFromPodcasts(podcastFeeds);

            PutPodcastFilesInDesinationDirectories(downloadedPodcastFilesToProcess);

            var playlistConfiguration = PlaylistConfigs.BuildFromConfigs();

            UpdatePlaylistsWithPlacedFiles(downloadedPodcastFilesToProcess, playlistConfiguration);

            Log.Debug("Completed processing all podcasts!");
        }

        private static void CreateAllRequiredDirectories()
        {
            Directories.CreateDirectoryIfNotExists(StringConstants.Mp3DownloadDirectory);
            Directories.CreateDirectoryIfNotExists(StringConstants.Mp3LogsDirectory);
            Directories.CreateDirectoryIfNotExists(StringConstants.Mp3MusicDirectory);
            Directories.CreateDirectoryIfNotExists(StringConstants.Mp3PlaylistsDirectory);
        }

        private static List<IRetrievablePodcast> GetPodcastFeeds()
        {
            var pathToFeedFile = string.Format(@"{0}\feeds.json", Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
            var feedReaderService = new FeedReaderService();
            var feeds = feedReaderService.GetFeeds(pathToFeedFile);

            return feeds;
        }

        private static List<FRPodcastFileToProcess> GetDownloadedFilesFromPodcasts(List<IRetrievablePodcast> podcasts)
        {
            // clean the diretory of any partially downloaded files
            FileOperations.DeleteAllFilesInDirectory(StringConstants.Mp3DownloadDirectory);

            var feedRetrievalService = new FeedRetrievalService();

            var downloadedPodcastsToProcess = feedRetrievalService.GetPodcastFilesForProcessing(podcasts);
            
            return downloadedPodcastsToProcess;
        }

        private static void PutPodcastFilesInDesinationDirectories(List<FRPodcastFileToProcess> downloadedPodcasts)
        {
            foreach (var podcastFile in downloadedPodcasts)
            {
                var fromPath = podcastFile.PathToDownloadedMp3;
                var toPath = podcastFile.DestinationPathForMp3;

                FileOperations.MoveFile(fromPath, toPath);
            }
        }

        private static void UpdatePlaylistsWithPlacedFiles(
            List<FRPodcastFileToProcess> downloadedPodcasts, 
            PlaylistConfigs playlistConfigs)
        {
            var distinctPlaylists = downloadedPodcasts.SelectMany(item => item.PlaylistPathsToIncludeIn)
                                                      .Distinct()
                                                      .ToList();
           
            // wsh: foreach playlist, get the mp3s that belong on it, read the current playlist, add them if they aren't there
            foreach(var playlist in distinctPlaylists)
            {
                var mp3sForPlaylist = downloadedPodcasts.Where(file => file.PlaylistPathsToIncludeIn.Contains(playlist))
                                                                                                    .ToList();

                AddMp3sToPlaylist(playlist, mp3sForPlaylist, playlistConfigs.MaxInnewpodcastsPlaylist);
            }
        }
        
        private static void AddMp3sToPlaylist(
            string playlist, 
            List<FRPodcastFileToProcess> mp3sForPlaylist, 
            int maxInnewpodcastsPlaylist)
        {
            PlaylistHelper.WriteNewPlaylist(playlist, mp3sForPlaylist, maxInnewpodcastsPlaylist);
        }

        private static void InitializeLogger()
        {
            log4net.Config.BasicConfigurator.Configure();

            Log.Debug("++++++++++++++++++++++++++++++++++++++");
            Log.Debug("FeedRefresher {0} starting...", AppVersion);
            Log.Debug("Current time UTC: '{0}'", DateTime.UtcNow.ToString("u"));
            Log.Debug("Current directory is: '{0}'", Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
            Log.Debug("--------------------------------------------------------------");
            Log.Debug("Feed Refresher Radio Updater - Podcast Retrievel and Placement Task");
            Log.Debug("Feed Refresher | 2016");
            Log.Debug("--------------------------------------------------------------");
        }
    }
}