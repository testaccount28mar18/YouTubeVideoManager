using System;
using System.Windows.Forms;
using YoutubeExtractor;

namespace YouTubeVideoManager
{
    /// <summary>
    /// Manages the progress bar for downloading videos
    /// </summary>
    public partial class frmDownloadProgressBar : Form
    {
        public frmDownloadProgressBar()
        {
            InitializeComponent();
        }

        private void frmDownloadProgressBar_Load(object sender, EventArgs e)
        {

        }

        public void Downloader_DownloadProgressChanged(object sender, ProgressEventArgs e)
        {
            //Update progressbar
            Invoke(new MethodInvoker(delegate () 
            {
                PBar.Value = (int)e.ProgressPercentage;
                lblPercentage.Text = $"{string.Format("{0:0.##}", e.ProgressPercentage)}%"; // update the lable on the form
                PBar.Update();
                if (e.ProgressPercentage == 100) // when loading is complete let the form be closed
                    brnClose.Enabled = true;
            }));
        }

        private void brnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
