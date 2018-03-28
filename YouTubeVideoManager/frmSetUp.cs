using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YouTubeVideoManager
{
    public partial class frmSetUp : Form
    {
        public frmSetUp()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (comboBoxQuality.Text != "") //If the box has a quality then the program can contine, VALIDATION
            {
                try
                {
                    Directory.CreateDirectory(txtPath.Text); // try making a folder in the location selected
                    Directory.CreateDirectory(txtPath.Text + @"\OfflineVideos");
                    Directory.CreateDirectory(txtPath.Text + @"\Settings");
                    Properties.Settings.Default.StorageLocation = txtPath.Text;
                    Properties.Settings.Default.VideoQuality = comboBoxQuality.Text;
                    DialogResult = DialogResult.OK;
                }
                catch // if there are any errors its most probabliy a permision error so selecting a new folder will help
                {
                    MessageBox.Show("Error creating folders needed. Please try a new directory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else // validation for quality
                MessageBox.Show("Please select a video quality", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnSelectLocation_Click(object sender, EventArgs e)
        {
            //Show a folder selection dialog to select a save path
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtPath.Text = folderBrowserDialog.SelectedPath + @"\YouTubeVideoManager";
            }
        }

        private void frmSetUp_Load(object sender, EventArgs e)
        { 
            // set the defalt save path as a folder in the documents
            txtPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\YouTubeVideoManager";
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("YouTubeVideoManagerHelp.chm");
        }
    }
}
