namespace YouTubeVideoManager
{
    partial class frmDownloadProgressBar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDownloadProgressBar));
            this.PBar = new System.Windows.Forms.ProgressBar();
            this.lblPercentage = new System.Windows.Forms.Label();
            this.brnClose = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PBar
            // 
            this.PBar.Location = new System.Drawing.Point(21, 44);
            this.PBar.Name = "PBar";
            this.PBar.Size = new System.Drawing.Size(241, 23);
            this.PBar.TabIndex = 0;
            // 
            // lblPercentage
            // 
            this.lblPercentage.AutoSize = true;
            this.lblPercentage.Location = new System.Drawing.Point(267, 49);
            this.lblPercentage.Name = "lblPercentage";
            this.lblPercentage.Size = new System.Drawing.Size(42, 13);
            this.lblPercentage.TabIndex = 1;
            this.lblPercentage.Text = "00.00%";
            // 
            // brnClose
            // 
            this.brnClose.Enabled = false;
            this.brnClose.Location = new System.Drawing.Point(124, 78);
            this.brnClose.Name = "brnClose";
            this.brnClose.Size = new System.Drawing.Size(75, 23);
            this.brnClose.TabIndex = 2;
            this.brnClose.Text = "Close";
            this.brnClose.UseVisualStyleBackColor = true;
            this.brnClose.Click += new System.EventHandler(this.brnClose_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(17, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(288, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Downloading Video, Please Wait....";
            // 
            // frmDownloadProgressBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(323, 106);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.brnClose);
            this.Controls.Add(this.lblPercentage);
            this.Controls.Add(this.PBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDownloadProgressBar";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Downloading";
            this.Load += new System.EventHandler(this.frmDownloadProgressBar_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button brnClose;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar PBar;
        private System.Windows.Forms.Label lblPercentage;
    }
}