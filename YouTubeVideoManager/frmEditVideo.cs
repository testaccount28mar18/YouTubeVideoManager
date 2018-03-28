using System;
using System.Windows.Forms;

namespace YouTubeVideoManager
{
    /// <summary>
    /// Allows video to be eddited
    /// </summary>
    public partial class frmEditVideo : Form
    {
        static int VideoIndexToEdit;
        public frmEditVideo()
        {
            InitializeComponent();
            //Loads current details from video details
            VideoIndexToEdit = frmMainManager.currentindex;
            txtDescription.Text = frmMainManager.ListOfVideos[VideoIndexToEdit].VideoDescription;
            txtTitle.Text = frmMainManager.ListOfVideos[VideoIndexToEdit].VideoTitle;
            txtNotes.Text = frmMainManager.ListOfVideos[VideoIndexToEdit].VideoNotes;
            txtCreator.Text = frmMainManager.ListOfVideos[VideoIndexToEdit].VideoUploader;
            txtID.Text = frmMainManager.ListOfVideos[VideoIndexToEdit].VideoID;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //Save any eddited details back
            frmMainManager.ListOfVideos[VideoIndexToEdit].VideoDescription = txtDescription.Text;
            frmMainManager.ListOfVideos[VideoIndexToEdit].VideoTitle = txtTitle.Text;
            frmMainManager.ListOfVideos[VideoIndexToEdit].VideoNotes = txtNotes.Text;
            frmMainManager.ListOfVideos[VideoIndexToEdit].VideoUploader = txtCreator.Text;
            DialogResult = DialogResult.OK;
        }

        private void frmEditVideo_Load(object sender, EventArgs e)
        {

        }
    }
}
