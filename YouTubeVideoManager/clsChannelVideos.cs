using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//ADD COMMENTS
namespace YouTubeVideoManager
{
    class clsChannelVideos
    {
        [JsonProperty("items")]
        public List<Item> Items { get; set; }

        public partial class Item
        {
            [JsonProperty("id")]
            public Id Id { get; set; }

            [JsonProperty("snippet")]
            public Snippet Snippet { get; set; }
        }

        public partial class Snippet
        {
            [JsonProperty("title")]
            public string Title { get; set; }
        }

        public partial class Id
        {
            [JsonProperty("videoId")]
            public string VideoId { get; set; }

            [JsonProperty("kind")]
            public string Kind { get; set; }
        }
    }
}
