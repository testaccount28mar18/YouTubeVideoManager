using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;


namespace YouTubeVideoManager
{
    /// <summary>
    /// Play the video online
    /// </summary>
    public partial class frmPlayOnline : Form
    {
        public frmPlayOnline()
        {
            InitializeComponent();
        }

        private void frmPlayOnline_Load(object sender, EventArgs e)
        {
            InitBrowser(); //initalise the browser
            frmMainManager.ListOfVideos[frmMainManager.currentindex].MarkWatched();
        }

        public ChromiumWebBrowser browser;
        public void InitBrowser()
        {
            if (!Cef.IsInitialized) // this is if this form is opened more than once in one session
                Cef.Initialize(new CefSettings());
            browser = new ChromiumWebBrowser("https://www.youtube.com/embed/"+ frmMainManager.ListOfVideos[frmMainManager.currentindex].VideoID + "?autoplay=1&loop=1"); //Points browser to video with certain settings
            this.Controls.Add(browser);//Add the controll
            browser.Dock = DockStyle.Fill;//Fill the form
        }
    }
}
