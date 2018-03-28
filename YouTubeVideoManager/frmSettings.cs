using System;
using System.Windows.Forms;

namespace YouTubeVideoManager
{
    /// <summary>
    /// Form to manage video quaility
    /// </summary>
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (comboBoxQuality.Text != "") // check quality is selected
            {
                Properties.Settings.Default.VideoQuality = comboBoxQuality.Text;
                DialogResult = DialogResult.OK;
            }
            else
                MessageBox.Show("Please select a video quality", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            comboBoxQuality.Text = Properties.Settings.Default.VideoQuality; // get current option
        }
    }
}
