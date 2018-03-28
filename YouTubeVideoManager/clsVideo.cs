using Newtonsoft.Json;
using System;
using System.Windows.Forms;
namespace YouTubeVideoManager
{
    /// <summary>
    /// Store all the video details
    /// </summary>
    [Serializable] //Allow to save the list object
    [JsonObject(MemberSerialization.OptIn)]
    public class clsVideo
    {
        //Attributes
        //All public due to the data view grid needing direct access
        [JsonProperty("id")]
        public string VideoID {get;set;}
        [JsonProperty("title")]
        public string VideoTitle { get; set; }
        [JsonProperty("publishedAt")]
        public DateTime VideoDate { get; set; }
        [JsonProperty("channelTitle")]
        public string VideoUploader { get; set; }
        [JsonProperty("description")]
        public string VideoDescription { get; set; }
        public string VideoNotes { get; set; }
        public bool VideoWatched { get; set; }
        public bool VideoDownloaded { get; set; }
        public string VideoDownloadURL;

        //Default Constructor
        public clsVideo()
        {
            VideoWatched = false;
            VideoDownloaded = false;
        }

        //OverLoad Constructor
        public clsVideo(string InVideoID, string InVideoUploader, string InVideoTitle, string InVideoDescription, DateTime InVideoDate, string InNotes)
        {
            try
            {
                VideoWatched = false;
                VideoDownloaded = false;
                VideoID = InVideoID;
                VideoUploader = InVideoUploader;
                VideoTitle = InVideoTitle;
                VideoDescription = InVideoDescription;
                VideoDate = InVideoDate;
                VideoNotes = InNotes;
            }
            catch(Exception e)
            {
                MessageBox.Show("Sorry, you've encounted an error! Message Follows:\n" + e.Message);
            }
        }

        /// <summary>
        /// Save the download url location and show that the video is downloaded
        /// </summary>
        /// <param name="VideoURL">Path to download</param>
        public void MarkDownloaded(string VideoURL)
        {
            VideoDownloaded = true;
            VideoDownloadURL = VideoURL;
        }

        /// <summary>
        /// Mark the video as watched
        /// </summary>
        public void MarkWatched()
        {
            VideoWatched = true;
        }

        /// <summary>
        /// Delete the downloaded copy of the video
        /// </summary>
        public void UnMarkDownloaded()
        {
            if (VideoDownloaded == true)
            {
                System.IO.File.Delete(Properties.Settings.Default.StorageLocation + @"\OfflineVideos\" + VideoID + ".mp4");
                VideoDownloaded = false;
                VideoDownloadURL = "";
            }
        }

        /// <summary>
        /// Mark a video as unwatched
        /// </summary>
        public void UnMarkWatched()
        {
            VideoWatched = false;
        }

        /// <summary>
        /// Update the notes of a video
        /// </summary>
        /// <param name="NewNotes">New string of notes</param>
        public void UpdateNotes(string NewNotes)
        {
            VideoNotes = NewNotes;
        }
    }
}
