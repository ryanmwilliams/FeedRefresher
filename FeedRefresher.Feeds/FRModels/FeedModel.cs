﻿using FeedRefresher.Feeds.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FeedRefresher.Feeds.FRModels
{
    

    public class Feed : IRetrievablePodcast
    {
        [JsonProperty(PropertyName = "feedId")]
        public int FeedId { get; set; }

        [JsonProperty(PropertyName = "maxFilesToDownload")]
        public int MaxFilesToDownload { get; set; }

        [JsonProperty(PropertyName = "podcastUrl")]
        public Uri PodcastUrl { get; set; }

        [JsonProperty(PropertyName = "targetPlaylistPaths")]
        public List<string> TargetPlaylistPaths { get; set; }

        [JsonProperty(PropertyName = "filterTitlesOn")]
        public List<string> FilterTitlesOn { get; set; }

        [JsonProperty(PropertyName = "destinationDirectory")]
        public string DestinationDirectory { get; set; }
    }

    [JsonObject]
    public class RssFeed
    {
        [JsonProperty(PropertyName = "feeds")]
        public List<Feed> Feeds { get; set; }
    }
}
