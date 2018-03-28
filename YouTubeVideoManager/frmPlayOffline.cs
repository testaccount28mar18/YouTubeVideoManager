using System;
using System.Windows.Forms;

namespace YouTubeVideoManager
{
    /// <summary>
    /// Form to play the video offline
    /// </summary>
    public partial class frmPlayOffline : Form
    {
        public frmPlayOffline()
        {
            InitializeComponent();
        }

        private void frmPlayOffline_Load(object sender, EventArgs e)
        {
            //load the video
            vlcControl1.Play(new Uri(Properties.Settings.Default.StorageLocation + @"\OfflineVideos\" + frmMainManager.ListOfVideos[frmMainManager.currentindex].VideoID + ".mp4"));
            frmMainManager.ListOfVideos[frmMainManager.currentindex].MarkWatched(); //mark video watched
        }

        private void frmPlayOffline_FormClosing(object sender, FormClosingEventArgs e)
        {
            vlcControl1.EndInit(); // closes the vlc instance when the form is closing
        }
    }
}
