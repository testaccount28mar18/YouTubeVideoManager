using System;
using System.Windows.Forms;

namespace YouTubeVideoManager
{
    /// <summary>
    /// View details for selected video
    /// </summary>
    public partial class frmViewVideoDetails : Form
    {
        static int VideoIndexToEdit;
        public frmViewVideoDetails()
        {
            InitializeComponent();
        }

        private void frmViewVideoDetails_Load(object sender, EventArgs e)
        {
            //Fill fields from the video
            VideoIndexToEdit = frmMainManager.currentindex;
            txtDescription.Text = frmMainManager.ListOfVideos[VideoIndexToEdit].VideoDescription;
            txtTitle.Text = frmMainManager.ListOfVideos[VideoIndexToEdit].VideoTitle;
            txtNotes.Text = frmMainManager.ListOfVideos[VideoIndexToEdit].VideoNotes;
            txtCreator.Text = frmMainManager.ListOfVideos[VideoIndexToEdit].VideoUploader;
            txtID.Text = frmMainManager.ListOfVideos[VideoIndexToEdit].VideoID;
            if (frmMainManager.ListOfVideos[VideoIndexToEdit].VideoDownloaded == true) //Make output pretty
            {
                txtDownloaded.Text = "Yes";
                txtDownloadLocation.Text = frmMainManager.ListOfVideos[VideoIndexToEdit].VideoDownloadURL;
            }
            else
            {
                txtDownloaded.Text = "No";
                txtDownloadLocation.Text = "N/a";
            }

            if (frmMainManager.ListOfVideos[VideoIndexToEdit].VideoWatched == true)
                txtWatched.Text = "Yes";
            else
                txtWatched.Text = "No";
        }
    }
}
