using System;
using System.Windows.Forms;

namespace YouTubeVideoManager
{
    /// <summary>
    /// Form allows user to enter search query
    /// </summary>
    public partial class frmSearch : Form
    {
        static public string SearchString { get; set; }
        public frmSearch()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchString = txtSearch.Text; // Save the text and send accep back to main form
            DialogResult = DialogResult.OK;
        }
    }
}
