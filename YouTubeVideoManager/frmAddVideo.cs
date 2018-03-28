using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace YouTubeVideoManager
{
    public partial class frmAddVideo : Form
    {
        /// <summary>
        /// Used to format the new video object before adding to list
        /// </summary>
        clsVideo TempVideoToAdd;
        private string APIKEY = Properties.Settings.Default.APIKey;
        public frmAddVideo()
        {
            InitializeComponent();
            //stop user adding nothing
            btnAdd.Enabled = false;
            btnGetDetails.Enabled = true;
            btnWhatIsID.Enabled = true;
        }

        /// <summary>
        /// Fill details about the video
        /// </summary>
        private async void btnGetDetails_Click(object sender, EventArgs e)
        {
            txtID.Text = txtID.Text.Trim(); //clear any spaces
            if (frmMainManager.ListOfVideos.Where(o => o.VideoID == txtID.Text).Any() == false) //make sure there are no duplicates VALIDATION
            {
                frmLoadingBar frmLoadingBar = new frmLoadingBar(); //show loading bar
                BeginInvoke((Action)(() => frmLoadingBar.ShowDialog()));
                string responseString = "";
                try
                {
                    Task<string> download = Task.Run(() => DownloadJSON("https://www.googleapis.com/youtube/v3/videos?id=" + txtID.Text + "&key=" + APIKEY + "&part=snippet&fields=items(id,snippet(title,description,channelTitle,publishedAt))"));
                    responseString = await download;
                    frmLoadingBar.Close();
                    if (!(responseString == "{\n \"items\": []\n}\n")) //format the strings VALIDATION means there is a video responded
                    {
                        responseString = responseString.Remove(0, 14);
                        responseString = responseString.Replace("   \"snippet\": {\n   ", "");
                        responseString = responseString.Remove(responseString.Length - 10, 10);
                        TempVideoToAdd = JsonConvert.DeserializeObject<clsVideo>(responseString);
                        txtCreator.Text = TempVideoToAdd.VideoUploader;
                        TempVideoToAdd.VideoDescription = TempVideoToAdd.VideoDescription.Replace("\n", System.Environment.NewLine);
                        txtDescription.Text = TempVideoToAdd.VideoDescription;
                        txtTitle.Text = TempVideoToAdd.VideoTitle;
                        //allow user to add video
                        btnAdd.Enabled = true;
                        txtID.Enabled = false;
                        btnGetDetails.Enabled = false;
                        btnWhatIsID.Enabled = false;
                    }
                    else // no video found
                    {
                        MessageBox.Show("There has been an error with this Video ID. No video has been found with this ID", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch
                {
                    frmLoadingBar.Close();
                    MessageBox.Show("There has been an error reaching YouTube. Please check your connection and try again", "Internet Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("There has already been a video added with this ID. Please add a new video.", "Exisiting Video", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Downloads a url and returns
        /// </summary>
        /// <param name="url">url to download</param>
        /// <returns>Return result string</returns>
        static public async Task<string> DownloadJSON(string url)
        {
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            wc.UseDefaultCredentials = true;
            string result = wc.DownloadString(new Uri(url));
            return result;
        }

        /// <summary>
        /// Add the object to the main list
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            TempVideoToAdd.VideoNotes = txtNotes.Text;
            frmMainManager.ListOfVideos.Add(TempVideoToAdd);
        }

        /// <summary>
        /// Show how to add
        /// </summary>
        private void btnWhatIsID_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "YouTubeVideoManagerHelp.chm", HelpNavigator.Topic, "AddingIndividualVideos.htm");
        }
    }
}