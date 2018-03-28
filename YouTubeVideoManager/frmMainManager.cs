using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using YoutubeExtractor;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Diagnostics;

namespace YouTubeVideoManager
{
    /// <summary>
    /// Main form when application loads.
    /// </summary>
    public partial class frmMainManager : Form
    {
        /// <summary>
        /// List stores all the video information objects. Public due to video info needing to be accessed from all forms
        /// </summary>
        public static List<clsVideo> ListOfVideos = new List<clsVideo>();

        /// <summary>
        /// Used to transfer selected video between forms for interaction
        /// </summary>
        public static int currentindex;

        /// <summary>
        /// During importing and resetting the application saving needs to be skiped. This controls that
        /// </summary>
        static bool SkipSave = false;

        /// <summary>
        /// Form Initialization
        /// </summary>
        public frmMainManager()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Checks that setup has been ran
        /// If setup has been ran and storage is condigured then load exisiting videos
        /// Update display with loaded videos
        /// </summary>
        private void MainScreen_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.SetUpRan == false)
            {
                //Sets up storage location for videos and files
                frmSetUp frmSetUp = new frmSetUp();
                DialogResult result = frmSetUp.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Properties.Settings.Default.SetUpRan = true; // Save setup is ran
                    Properties.Settings.Default.Save();
                }
                else // setup exited before being complete
                {
                    MessageBox.Show("Error setting up the program. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
            if (File.Exists(Properties.Settings.Default.StorageLocation + @"\Settings\settings.YTMDB")) //load the settings file if it exists
                ListOfVideos = DeserializeData<clsVideo>(Properties.Settings.Default.StorageLocation + @"\Settings\settings.YTMDB"); //Fill the list with the saved videos
            UpdateDataGrid(ListOfVideos);
        }

        /// <summary>
        /// Serializes data to a file in storage location
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">A clsVideo list that will be serilized</param>
        /// <param name="SerializationFile">Location of output file</param>
        public void SerializeData<T>(ref List<T> list, String SerializationFile)
        {
            using (Stream stream = File.Open(SerializationFile, FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bformatter.Serialize(stream, list);
            }
        }

        /// <summary>
        /// Loads the serilized file into a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SerializationFile">Location of file to be deserilized</param>
        /// <returns>Returns a list of videos that will be loaded</returns>
        public List<T> DeserializeData<T>(String SerializationFile)
        {
            using (Stream stream = File.Open(SerializationFile, FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (List<T>)bformatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// This binds the list of videos to the data view gird.
        /// Buttons ae then disabled - this is as there is no video selected (when the video is selected they get enabled)
        /// If the list is empty then more buttons need to be disabled to prevent actions that cannot be preformed
        /// </summary>
        /// <param name="list">List of videos with the clsVideo type that will be displayed</param>
        private void UpdateDataGrid(List<clsVideo> list)
        {
            //Bind lists
            var bindingList = new BindingList<clsVideo>(list);
            var source = new BindingSource(bindingList, null);
            DataGrid.DataSource = source;
            DataGrid.ClearSelection(); // Make sure no video is selected
            //Disable buttons
            rbEdit.Enabled = false;
            rbRemoveVideo.Enabled = false;
            rbRemoveChannel.Enabled = false;
            rbPlaySelected.Enabled = false;
            rbMarkUnwatched.Enabled = false;
            rbMarkWatched.Enabled = false;
            rbDownloadSelected.Enabled = false;
            rbUndownloadSelected.Enabled = false;
            rbViewDetails.Enabled = false;
            rbPlayDownloaded.Enabled = false;
            rbPlayRandom.Enabled = true;
            rbDeleteDownloaded.Enabled = true;
            rbSortCreator.Enabled = true;
            rbSortDate.Enabled = true;
            rbSortDownloaded.Enabled = true;
            rbSortID.Enabled = true;
            rbSortName.Enabled = true;
            rbSortWatched.Enabled = true;
            if (!list.Any())
            {
                rbSortCreator.Enabled = false;
                rbSortDate.Enabled = false;
                rbSortDownloaded.Enabled = false;
                rbSortID.Enabled = false;
                rbSortName.Enabled = false;
                rbSortWatched.Enabled = false;
                rbPlayRandom.Enabled = false;
                rbDeleteDownloaded.Enabled = false;
            }
        }

        /// <summary>
        /// Add channel to database
        /// </summary>
        private void rbAddChannel_Click(object sender, EventArgs e)
        {
            frmAddChannel frmAddChannel = new frmAddChannel();
            if (frmAddChannel.ShowDialog() == DialogResult.OK)
                UpdateDataGrid(ListOfVideos); //If new video has been added then update the display
        }

        /// <summary>
        /// Adds a video by opening a new form. If the video is added then update the display
        /// </summary>
        private void rbAddVideo_Click(object sender, EventArgs e)
        {
            //Shows the add video form
            frmAddVideo frmAddVideo = new frmAddVideo();
            if (frmAddVideo.ShowDialog() == DialogResult.OK)
                UpdateDataGrid(ListOfVideos); //If new video has been added then update the display
        }

        /// <summary>
        /// Shows a form with all the details about current selected video
        /// </summary>
        private void rbViewDetails_Click(object sender, EventArgs e)
        {
            if (ListOfVideos.Any()) // make sure list has something in before I try and load something that isnt there
            {
                string VideoIDToEdit = DataGrid.CurrentRow.Cells["colVideoID"].Value.ToString(); // get current id
                int index = ListOfVideos.FindIndex(video => video.VideoID == VideoIDToEdit); // find index
                currentindex = index; // share index
                frmViewVideoDetails frmViewVideoDetails = new frmViewVideoDetails();
                frmViewVideoDetails.ShowDialog(); // load form
            }
        }

        /// <summary>
        /// Allows editting of the video apart from video ID
        /// </summary>
        private void rbEdit_Click(object sender, EventArgs e)
        {
            if (ListOfVideos.Any()) // make sure list has something in before I try and delete something that isnt there because that isnt good and this is now a long comment
            {
                //Get ID ect
                string VideoIDToEdit = DataGrid.CurrentRow.Cells["colVideoID"].Value.ToString();
                int index = ListOfVideos.FindIndex(video => video.VideoID == VideoIDToEdit);
                currentindex = index;
                frmEditVideo frmEditVideo = new frmEditVideo();
                if (frmEditVideo.ShowDialog() == DialogResult.OK) //if edit has been made then update the changes
                {
                    UpdateDataGrid(ListOfVideos);
                }
            }
        }

        /// <summary>
        /// Removes selected video from the list and if required deletes any downloaded files
        /// </summary>
        private void rbRemoveVideo_Click(object sender, EventArgs e)
        {
            DialogResult ans = MessageBox.Show("Are you sure you would like to remove this video? The video and any offline copies will be deleted.",
                "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation); // check they want to remove it
            if (ans == DialogResult.Yes)
            {
                if (ListOfVideos.Any()) // make sure list has something in before I try and delete something that isnt there
                {
                    string VideoIDToRemove = DataGrid.CurrentRow.Cells["colVideoID"].Value.ToString(); // Get the id that is currently selected
                    int index = ListOfVideos.FindIndex(video => video.VideoID == VideoIDToRemove); // find the list index from this id
                    ListOfVideos[index].UnMarkDownloaded(); // delete any downloaded files
                    ListOfVideos.RemoveAt(index); //remove items from the list now possbile downloads are deleted
                    UpdateDataGrid(ListOfVideos); // update display
                    MessageBox.Show("Video has been removed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Remove any videos of the selected video channel
        /// </summary>
        private void rbRemoveChannel_Click(object sender, EventArgs e)
        {
            DialogResult ans = MessageBox.Show("Are you sure you would like to remove all videos from this Creator? Any offline copies will also be removed.", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (ans == DialogResult.Yes)
            {
                if (ListOfVideos.Any()) // make sure list has something in before I try and delete something that isnt there
                {
                    int counter = 0; // counts how many videos have been removed
                    string VideoCreatorToRemove = DataGrid.CurrentRow.Cells["colCreator"].Value.ToString(); // get the creator name
                    foreach (clsVideo vid in ListOfVideos.Where(o => o.VideoUploader == VideoCreatorToRemove))//delete any downloaded videos
                    {
                        counter++; // add one to deleted
                        vid.UnMarkDownloaded(); // unload any deleted files
                    }
                    ListOfVideos.RemoveAll(video => video.VideoUploader == VideoCreatorToRemove); //remove items from the list now possbile downloads are deleted
                    UpdateDataGrid(ListOfVideos); // update display
                    MessageBox.Show(counter.ToString() + " video/videos have been removed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Downloads the selected video with the quaility setting. If quality isn't avalible then get the best one possbile
        /// </summary>
        private void rbDownloadSelected_Click(object sender, EventArgs e)
        {

            //start progrss bar form
            frmDownloadProgressBar frmDownloadProgressBar = new frmDownloadProgressBar();
            try
            {
                //gets id to download
                string VideoIDToDL = DataGrid.CurrentRow.Cells["colVideoID"].Value.ToString();
                int index = ListOfVideos.FindIndex(o => o.VideoID == VideoIDToDL);
                if (ListOfVideos[index].VideoDownloaded == false) // check that the video is not downloaded
                {
                    //Get video information for downloaded
                    IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls("https://www.youtube.com/watch?v=" + VideoIDToDL);
                    IEnumerable<VideoInfo> v = videoInfos;

                    //Set qualility
                    var video = v.FirstOrDefault(info => (info.VideoType == VideoType.Mp4) && (info.Resolution == Convert.ToInt16(Properties.Settings.Default.VideoQuality)));

                    //If no youtube video is found with the quality that the user wants, select the best quality from the list
                    if (video == null)
                    {
                        video = v
                            .OrderByDescending(x => x.Resolution)
                            .First(info => info.VideoType == VideoType.Mp4);

                    }

                    //decrypt if needed
                    if (video.RequiresDecryption)
                    {
                        DownloadUrlResolver.DecryptDownloadUrl(video);
                    }

                    //set download location
                    var videoDownloader = new VideoDownloader(video, Path.Combine(Properties.Settings.Default.StorageLocation + @"\OfflineVideos", VideoIDToDL + video.VideoExtension));

                    //links progressbar progress event to progress form
                    videoDownloader.DownloadProgressChanged += frmDownloadProgressBar.Downloader_DownloadProgressChanged;

                    //Create a new thread to download file
                    Thread thread = new Thread(() => { videoDownloader.Execute(); }) { IsBackground = true };
                    thread.Start();
                    //show the progress bar
                    frmDownloadProgressBar.ShowDialog();
                    ListOfVideos[index].MarkDownloaded(videoDownloader.SavePath); //mark the video downloaded and store the path
                    UpdateDataGrid(ListOfVideos);
                }
                else
                {
                    MessageBox.Show("Cannot download video as it is already downloaded!", "Video Not Downloaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                frmDownloadProgressBar.Close();
                MessageBox.Show("There has been an error downloading the video:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Deletes the downloaded copy of selected video
        /// </summary>
        private void rbUndownloadSelected_Click(object sender, EventArgs e)
        {
            //Get ID
            string VideoIDToShow = DataGrid.CurrentRow.Cells["colVideoID"].Value.ToString();
            int index = ListOfVideos.FindIndex(video => video.VideoID == VideoIDToShow);
            if (ListOfVideos[index].VideoDownloaded == true) // Check video is downloaded
            {
                DialogResult ans = MessageBox.Show("Are you sure you would like to remove the offline copy of this video? The video will continue to be listed after the offline copy is removed.", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (ans == DialogResult.Yes)
                {
                    ListOfVideos[index].UnMarkDownloaded(); // Delete it
                    UpdateDataGrid(ListOfVideos);
                    MessageBox.Show("Offine copy has been removed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Cannot undownload video as it is not downloaded in the first place!", "Video Not Undownloaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Delete all the downloaded copies of videos
        /// </summary>
        private void rbDeleteDownloaded_Click(object sender, EventArgs e)
        {
            DialogResult ans = MessageBox.Show("Are you sure you would like to remove the offline copy of ALL videos? The videos will continue to be listed after the offline copy is removed.", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (ans == DialogResult.Yes)
            {
                int counter = 0; // Count how many are deleted
                foreach (clsVideo vid in ListOfVideos.Where(o => o.VideoDownloaded == true))
                {
                    vid.UnMarkDownloaded(); // delete video
                    counter++;
                }
                UpdateDataGrid(ListOfVideos); // update display
                MessageBox.Show(counter.ToString() + " offine copy/copies have been removed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Play selected video online
        /// </summary>
        private void rbPlaySelected_Click(object sender, EventArgs e)
        {
            if (ListOfVideos.Any()) // make sure list has something in before I try and play something that isnt there
            {
                //Get ID
                string VideoIDToShow = DataGrid.CurrentRow.Cells["colVideoID"].Value.ToString();
                int index = ListOfVideos.FindIndex(video => video.VideoID == VideoIDToShow);
                currentindex = index;
                frmPlayOnline frmPlayOnline = new frmPlayOnline();
                frmPlayOnline.Show();
                UpdateDataGrid(ListOfVideos); //Refresh display due play status being marked
            }
        }

        /// <summary>
        /// Play a random video online
        /// </summary>
        private void rbPlayRandom_Click(object sender, EventArgs e)
        {
            Random randomnumber = new Random(); // set up a random instance
            currentindex = randomnumber.Next(0, ListOfVideos.Count); // set up the current play index and generate a number with a max of number of videos
            frmPlayOnline frmPlayOnline = new frmPlayOnline();
            frmPlayOnline.Show();
        }

        /// <summary>
        /// Play a video offline from a downloaded copy
        /// </summary>
        private void rbPlayDownloaded_Click(object sender, EventArgs e)
        {
            //Get ID
            string VideoIDToShow = DataGrid.CurrentRow.Cells["colVideoID"].Value.ToString();
            int index = ListOfVideos.FindIndex(video => video.VideoID == VideoIDToShow);
            currentindex = index;
            if (ListOfVideos[currentindex].VideoDownloaded == true) //If it is downloaded then play it
            {
                frmPlayOffline frmPlayOffline = new frmPlayOffline();
                frmPlayOffline.Show();
                UpdateDataGrid(ListOfVideos);
            }
            else
            {
                MessageBox.Show("Please download the video before trying to play it offline.", "Video Not Downloaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Show the about dialog
        /// </summary>
        private void rbAbout_Click(object sender, EventArgs e)
        {
            frmAbout frmAbout = new frmAbout();
            frmAbout.ShowDialog();
        }

        /// <summary>
        /// Show all the videos in the data view grid
        /// </summary>
        private void rbShowAll_Click(object sender, EventArgs e)
        {
            UpdateDataGrid(ListOfVideos);
        }

        /// <summary>
        /// Hide all current videos - maybe the boss walked by?!
        /// </summary>
        private void rbHideAll_Click(object sender, EventArgs e)
        {
            List<clsVideo> list = new List<clsVideo>();
            UpdateDataGrid(list);
        }

        /// <summary>
        /// Refreshes the data view grid - for example to remove the filters/searches/sorts
        /// </summary>
        private void rbRefresh_Click(object sender, EventArgs e)
        {
            UpdateDataGrid(ListOfVideos); // Refreshes desplay with default list - removes any sorts etc
        }

        /// <summary>
        /// Any time a video is selected, enable the buttons related with that indivdual video
        /// </summary>
        private void DataGrid_SelectionChanged(object sender, EventArgs e)
        {
            rbEdit.Enabled = true;
            rbRemoveVideo.Enabled = true;
            rbRemoveChannel.Enabled = true;
            rbPlaySelected.Enabled = true;
            rbDownloadSelected.Enabled = true;
            rbViewDetails.Enabled = true;
            rbPlayDownloaded.Enabled = true;
            rbUndownloadSelected.Enabled = true;
            rbMarkWatched.Enabled = true;
            rbMarkUnwatched.Enabled = true;
        }

        /// <summary>
        /// If the video is double clicked, then show the details about that video
        /// </summary>
        private void DataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ListOfVideos.Any()) // make sure list has something in before I try and delete something that isnt there because that isnt good and this is now a long comment
            {
                // Get ID
                string VideoIDToEdit = DataGrid.CurrentRow.Cells["colVideoID"].Value.ToString();
                int index = ListOfVideos.FindIndex(video => video.VideoID == VideoIDToEdit);
                currentindex = index;
                frmViewVideoDetails frmViewVideoDetails = new frmViewVideoDetails();
                frmViewVideoDetails.ShowDialog(); // Show the video details dialog
            }
        }

        /// <summary>
        /// When the form closes save the video list to file for next open
        /// </summary>
        private void frmMainManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SkipSave == false)
                SerializeData<clsVideo>(ref ListOfVideos, Properties.Settings.Default.StorageLocation + @"\Settings\settings.YTMDB");
        }

        // The following region handles all the sorts using simple lambda expressions
        #region Sorts and Searches

        private void rbSortNameA_Click(object sender, EventArgs e)
        {
            UpdateDataGrid(ListOfVideos.OrderBy(o => o.VideoTitle).ToList());
        }

        private void rbSortNameD_Click(object sender, EventArgs e)
        {
            UpdateDataGrid(ListOfVideos.OrderByDescending(o => o.VideoTitle).ToList());
        }

        private void rbSortIDA_Click(object sender, EventArgs e)
        {
            UpdateDataGrid(ListOfVideos.OrderBy(o => o.VideoID).ToList());
        }

        private void rbSortIDD_Click(object sender, EventArgs e)
        {
            UpdateDataGrid(ListOfVideos.OrderByDescending(o => o.VideoID).ToList());
        }

        private void rbSortCreatorA_Click(object sender, EventArgs e)
        {
            UpdateDataGrid(ListOfVideos.OrderBy(o => o.VideoUploader).ToList());
        }

        private void rbSortCreatorD_Click(object sender, EventArgs e)
        {
            UpdateDataGrid(ListOfVideos.OrderByDescending(o => o.VideoUploader).ToList());
        }

        private void rbSortDateA_Click(object sender, EventArgs e)
        {
            UpdateDataGrid(ListOfVideos.OrderBy(o => o.VideoDate).ToList());
        }

        private void rbSortDateD_Click(object sender, EventArgs e)
        {
            UpdateDataGrid(ListOfVideos.OrderByDescending(o => o.VideoDate).ToList());
        }

        private void rbSortWatchedA_Click(object sender, EventArgs e)
        {
            UpdateDataGrid(ListOfVideos.OrderBy(o => o.VideoWatched).ToList());
        }

        private void rbSortWatchedD_Click(object sender, EventArgs e)
        {
            UpdateDataGrid(ListOfVideos.OrderByDescending(o => o.VideoWatched).ToList());
        }

        private void rbSortDownloadA_Click(object sender, EventArgs e)
        {
            UpdateDataGrid(ListOfVideos.OrderBy(o => o.VideoDownloaded).ToList());
        }

        private void rbSortDownloadD_Click(object sender, EventArgs e)
        {
            UpdateDataGrid(ListOfVideos.OrderByDescending(o => o.VideoDownloaded).ToList());
        }

        private void rbSearch_Click(object sender, EventArgs e)
        {
            frmSearch frmSearch = new frmSearch();
            List<clsVideo> SearchList = new List<clsVideo>();
            if (frmSearch.ShowDialog() == DialogResult.OK)
            {
                string search = frmSearch.SearchString.ToUpper(); // Get the search string from the form
                foreach (clsVideo vid in ListOfVideos) // go through each video
                {
                    if (vid.VideoTitle.ToUpper().Contains(search) || vid.VideoDescription.ToUpper().Contains(search) ||
                        vid.VideoUploader.ToUpper().Contains(search) || vid.VideoID.ToUpper().Contains(search) ||
                        vid.VideoDate.ToString().ToUpper().Contains(search)) // if the video title, desription, uploader, ID or date contains the search string add it to a list
                    {
                        SearchList.Add(vid);
                    }
                }
                UpdateDataGrid(SearchList); // display the new list
            }
        }

        #endregion

        /// <summary>
        /// Mark selected video as watched
        /// </summary>
        private void rbMarkWatched_Click(object sender, EventArgs e)
        {
            //Get ID
            string VideoIDToShow = DataGrid.CurrentRow.Cells["colVideoID"].Value.ToString();
            int index = ListOfVideos.FindIndex(video => video.VideoID == VideoIDToShow);
            ListOfVideos[index].MarkWatched(); //Mark watched
            UpdateDataGrid(ListOfVideos); //Update
        }

        /// <summary>
        /// Mark selected video as unwatched
        /// </summary>
        private void rbMarkUnwatched_Click(object sender, EventArgs e)
        {
            //Get ID
            string VideoIDToShow = DataGrid.CurrentRow.Cells["colVideoID"].Value.ToString();
            int index = ListOfVideos.FindIndex(video => video.VideoID == VideoIDToShow);
            ListOfVideos[index].UnMarkWatched(); //Mark unwatched
            UpdateDataGrid(ListOfVideos);
        }

        /// <summary>
        /// Save the current video list to settings file
        /// </summary>
        private void rbSaveNow_Click(object sender, EventArgs e)
        {
            SerializeData<clsVideo>(ref ListOfVideos, Properties.Settings.Default.StorageLocation + @"\Settings\settings.YTMDB");
            MessageBox.Show("Database Saved! Please note, this automatically happens when you close the program.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Open settings dialog
        /// </summary>
        private void rbSettings_Click(object sender, EventArgs e)
        {
            frmSettings frmSettings = new frmSettings();
            frmSettings.ShowDialog();
        }

        /// <summary>
        /// Delete all downloaded videos and reset all settings and lists
        /// </summary>
        private void rbReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure? \n\nThis will delete all downloaded videos and remove the Database there is no recovery for this.\n\nThe application will restart after this.", "WARNGING", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                DirectoryInfo di = new DirectoryInfo(Properties.Settings.Default.StorageLocation);
                foreach (FileInfo file in di.GetFiles()) // delete every file in storage location
                    file.Delete();
                foreach (DirectoryInfo dir in di.GetDirectories()) // delete every folder in storage location
                    dir.Delete(true);
                Directory.Delete(Properties.Settings.Default.StorageLocation, true); // delete storage location
                Properties.Settings.Default.Reset(); // reset application settings
                SkipSave = true; // dont save the video list on next launch
                Application.Restart(); // restart the program
            }
        }

        /// <summary>
        /// Export all downloaded videos and list of videos into one file
        /// </summary>
        private void rbExport_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("This will make a file that copys all downloaded videos and the database into one single file for backup or transfer.\n\nIn the next screen select the location for the exported file.", "Export Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SaveFileDialog SD = new SaveFileDialog(); // Make a save dialog with certain properties
                SD.Filter = "YouTube Manager Export|*.YTME";
                SD.Title = "Export File Save Location";
                SD.FileName = "ExportedFile";
                SD.ShowDialog();
                if (SD.FileName != "ExportedFile") // if the file name contains something then we can save the export file
                {
                    SerializeData<clsVideo>(ref ListOfVideos, Properties.Settings.Default.StorageLocation + @"\Settings\settings.YTMDB"); // save the video list to encounter any changes
                    File.Delete(SD.FileName); //if a file needs to be replaced then we need to delete it first the open dialog already asks if you want to replace the file
                    ZipFile.CreateFromDirectory(Properties.Settings.Default.StorageLocation, SD.FileName); // make a zip folder with custom extension with storage location
                }
            }
            catch (Exception ex) // catch any execptions that might occur - dont know what might happen here so playing safe
            {
                MessageBox.Show("Error exporting. Please try a new export file location/name or contact devs with the message:\n" +ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Imports from file and clears anything currently stored
        /// </summary>
        private void rbImportData_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("This will coppy all data from the exported file and will replace any current data.\n\nIn the next screen select the file to import.", "Import Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OpenFileDialog OF = new OpenFileDialog(); // Open a file with selected properties
                OF.Filter = "YouTube Manager Export|*.YTME";
                OF.Title = "Import File";
                OF.ShowDialog();
                if (OF.FileName != "") // if the name contanins something start the import
                {
                    //delete everything currently there
                    DirectoryInfo di = new DirectoryInfo(Properties.Settings.Default.StorageLocation);
                    foreach (FileInfo file in di.GetFiles())
                        file.Delete();
                    foreach (DirectoryInfo dir in di.GetDirectories())
                        dir.Delete(true);
                    Directory.Delete(Properties.Settings.Default.StorageLocation, true);
                    Directory.CreateDirectory(Properties.Settings.Default.StorageLocation);
                    ZipFile.ExtractToDirectory(OF.FileName, Properties.Settings.Default.StorageLocation); // extract video to that folder
                    SkipSave = true; // dont save
                    Application.Restart(); //restart
                }
            }
            catch (Exception ex)// catch any execptions that might occur - dont know what might happen here so playing safe
            {
                MessageBox.Show("Error importing. Please try a new export file or contact devs with the message:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Opens help dialog
        /// </summary>
        private void rbHelp_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "YouTubeVideoManagerHelp.chm");
        }

        /// <summary>
        /// If F1 is clicked or help is requested through an assessibility program the help will show
        /// </summary>
        private void frmMainManager_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Help.ShowHelp(this, "YouTubeVideoManagerHelp.chm");
        }

        /// <summary>
        /// Opens the default browser so users can find videos to add
        /// </summary>
        private void rbYouTube_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.youtube.com/");
        }
    }
}