using Newtonsoft.Json;
using System.Collections.Generic;
namespace YouTubeVideoManager
{
    //test comment
    /// <summary>
    /// This is used to store channel information while adding videos by a channel
    /// </summary>
    public partial class clsChannel
    {
        [JsonProperty("pageInfo")]
        public PageInfo PageInfo { get; set; }

        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }

    /// <summary>
    /// Part of the channel class, stores information in the channel
    /// </summary>
    public partial class Item
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("snippet")]
        public Snippet Snippet { get; set; }
    }

    /// <summary>
    /// Stores the channel title
    /// </summary>
    public partial class Snippet
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    }

    /// <summary>
    /// Stores how many results there are for the channnel
    /// </summary>
    public partial class PageInfo
    {
        [JsonProperty("totalResults")]
        public long TotalResults { get; set; }
    }
}