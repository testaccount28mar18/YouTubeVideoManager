using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace YouTubeVideoManager
{
    public partial class frmAddChannel : Form
    {
        private string APIKEY = Properties.Settings.Default.APIKey;
        /// <summary>
        /// Temp store each video in a channel until it is added to the main list
        /// </summary>
        clsChannelVideos ChannelVideoForList;

        /// <summary>
        /// Store the details about a channel while getting processed and videos are added
        /// </summary>
        clsChannel ChannelDetails;

        /// <summary>
        /// If a video needs to be added, it will be processed in this instance before being added to the main list for saftey
        /// </summary
        clsVideo VideoToAdd; 

        public frmAddChannel()
        {
            InitializeComponent();
            btnAdd.Enabled = false;
        }

        /// <summary>
        /// When the add button is clicked we need to get details for each video that the user wants to add to the application.
        /// This will download info about each vido then add to the main list
        /// </summary>
        private async void btnAdd_Click(object sender, EventArgs e)
        {
            frmLoadingBar frmLoadingBar = new frmLoadingBar();
            BeginInvoke((Action)(() => frmLoadingBar.ShowDialog())); //Show loading form
            try
            {
                foreach (object checkeditem in checkedListBox1.CheckedItems) //Each video that is been selected to add
                {
                    int index = ChannelVideoForList.Items.FindIndex(o => o.Snippet.Title == checkeditem.ToString()); //get the index of the selected video
                    if (frmMainManager.ListOfVideos.Where(o => o.VideoID == ChannelVideoForList.Items[index].Id.VideoId).Any() == false) // check it isnt already in the DB
                    {
                        string responseString = "";
                        Task<string> download = Task.Run(() => frmAddVideo.DownloadJSON("https://www.googleapis.com/youtube/v3/videos?id=" + ChannelVideoForList.Items[index].Id.VideoId + "&key=" + APIKEY + "&part=snippet&fields=items(id,snippet(title,description,channelTitle,publishedAt))"));
                        //Download the video JSON
                        responseString = await download;
                        responseString = responseString.Remove(0, 14);
                        responseString = responseString.Replace("   \"snippet\": {\n   ", ""); //Format JSOM
                        responseString = responseString.Remove(responseString.Length - 10, 10);
                        VideoToAdd = JsonConvert.DeserializeObject<clsVideo>(responseString);
                        VideoToAdd.VideoDescription = VideoToAdd.VideoDescription.Replace("\n", System.Environment.NewLine); //Replace any newlines
                        frmMainManager.ListOfVideos.Add(VideoToAdd); //Format complete add to list
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("There has been an error reaching YouTube. Please check your connection and try again.\n"+ex.Message, "Internet Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            frmLoadingBar.Close();
            DialogResult = DialogResult.OK; // All videos added close dialog
        }

        /// <summary>
        /// Get details about the channel name or ID then get a list of videos
        /// </summary>
        private async void btnGetDetails_Click(object sender, EventArgs e)
        {
            frmLoadingBar frmLoadingBar = new frmLoadingBar();
            BeginInvoke((Action)(() => frmLoadingBar.ShowDialog())); //Show loading form
            // get the channel id from a channel name
            try //try and catch catches any internet issues
            {
                Task<string> downloadchan = Task.Run(() => frmAddVideo.DownloadJSON("https://www.googleapis.com/youtube/v3/channels/?forUsername=" + txtID.Text + "&part=snippet&key=" + APIKEY));
                string responseString = await downloadchan;
                // store the temp channel details so we can get a channel id
                ChannelDetails = JsonConvert.DeserializeObject<clsChannel>(responseString);
                frmLoadingBar.Close();
                // if the totalresults is 1 it means we have a match therefore can get the videos with that id
                if (ChannelDetails.PageInfo.TotalResults == 1)
                {
                    FillComboKnowID(ChannelDetails.Items[0].Id);
                }
                // the txt entered could be a channel id already so we need to test for that
                else
                {
                    FillComboUserID(txtID.Text);
                }
            }
            catch (Exception ex)
            {
                frmLoadingBar.Close();
                MessageBox.Show("There has been an error reaching YouTube. Please check your connection and try again. \n" + ex.Message, "Internet Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// This is used when a channel name has been converted to an ID by the program so we know it is the correct ID.
        /// This will then get videos from the channel
        /// </summary>
        /// <param name="channelID">ChannelID</param>
        private async void FillComboKnowID(string channelID)
        {
            frmLoadingBar frmLoadingBar1 = new frmLoadingBar();
            BeginInvoke((Action)(() => frmLoadingBar1.ShowDialog()));
            try
            {
                Task<string> downloadvid = Task.Run(() => frmAddVideo.DownloadJSON("https://www.googleapis.com/youtube/v3/search?order=date&part=snippet&channelId=" + channelID + "&maxResults=50&key=" + APIKEY));
                string responseString = await downloadvid;
                frmLoadingBar1.Close();
                ChannelVideoForList = JsonConvert.DeserializeObject<clsChannelVideos>(responseString); //stores a list of the videos in the channel
                checkedListBox1.Items.Clear();
                if (ChannelVideoForList.Items.Any()) //check there are videos
                {
                    foreach (clsChannelVideos.Item vid in ChannelVideoForList.Items) // fill the combo box
                    {
                        if (vid.Id.Kind == "youtube#video") // check that the video is a video not a channel - fix an empty channel bug
                            checkedListBox1.Items.Add(vid.Snippet.Title);
                    }
                    //now there are videos we can let users add them and stop them changing anything else
                    btnAdd.Enabled = true;
                    btnGetDetails.Enabled = false;
                    btnWhatIsID.Enabled = false;
                    txtID.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                frmLoadingBar1.Close();
                MessageBox.Show("There has been an error reaching YouTube. Please check your connection and try again. \n" +ex.Message, "Internet Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //used when a channel id is entered not a channel name. must lookup channel id to get the name first
        /// <summary>
        /// Used when the user entered a channel id in the channel name box. We need to check that the ID is real.
        /// Will fill the combo box if its a good ID
        /// </summary>
        /// <param name="channelID"></param>
        private async void FillComboUserID(string channelID)
        {
            frmLoadingBar frmLoadingBar1 = new frmLoadingBar();
            BeginInvoke((Action)(() => frmLoadingBar1.ShowDialog()));
            try
            {
                Task<string> downloadchan = Task.Run(() => frmAddVideo.DownloadJSON("https://www.googleapis.com/youtube/v3/channels/?id=" + txtID.Text + "&part=snippet&key=" + APIKEY));
                string responseStringchan = await downloadchan;
                // store the temp channel details so we can get a channel id
                ChannelDetails = JsonConvert.DeserializeObject<clsChannel>(responseStringchan); 


                Task<string> downloadvid = Task.Run(() => frmAddVideo.DownloadJSON("https://www.googleapis.com/youtube/v3/search?order=date&part=snippet&channelId=" + channelID + "&maxResults=50&key=" + APIKEY));
                string responseString = await downloadvid;
                frmLoadingBar1.Close();
                ChannelVideoForList = JsonConvert.DeserializeObject<clsChannelVideos>(responseString); //get list of videos
                checkedListBox1.Items.Clear();
                if (ChannelVideoForList.Items.Any()) // fill the combo box if there is videos
                {
                    foreach (clsChannelVideos.Item vid in ChannelVideoForList.Items) //fill combo
                    {
                        if (vid.Id.Kind == "youtube#video") // make sure video
                            checkedListBox1.Items.Add(vid.Snippet.Title);
                    }
                    //allow user to continue
                    btnAdd.Enabled = true;
                    btnGetDetails.Enabled = false;
                    btnWhatIsID.Enabled = false;
                    txtID.Enabled = false;
                }
                else
                {
                    //if there are no videos then theres probably not a good channel id
                    MessageBox.Show("There has been an error with this Channel ID/Name. No channel has been found with this ID/Name", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (WebException e)
            {
                frmLoadingBar1.Close(); // if exits above get rid of the loading screen
                if (((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.BadRequest) //could be an ID
                {
                    MessageBox.Show("There has been an error with this Channel ID/Name. No channel has been found with this ID/Name", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("There has been an error reaching YouTube. Please check your connection and try again", "Internet Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Shows how to get info on getting ID
        /// </summary>
        private void btnWhatIsID_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "YouTubeVideoManagerHelp.chm", HelpNavigator.Topic, "AddingaChannel.htm");
        }
    }
}