using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace ApplicationUpdate
{
    internal partial class ApplicationUpdaterDownloadForm : Form
    {
        private WebClient webClient;
        private BackgroundWorker bgWorker;
        private string tempFile;
        

        internal string TempFilePath
        {
            get { return this.tempFile; }
        }

        private void webClient_DownloadProgressChanged(object s, DownloadProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
            this.lblProgress.Text = string.Format("Download {0} of {1}", FormatBytes(e.BytesReceived, 1, true), FormatBytes(e.TotalBytesToReceive, 1, true));
        }

        private string FormatBytes(long bytes, int decimalPlaces, bool showByteTypes)
        {
            double newBytes = bytes;
            string formatString = "{0";
            string byteType = "B";

            if(newBytes > 1024 && newBytes < 1048576)
            {
                newBytes /= 1024;
                byteType = "KB";
            }
            else if (newBytes > 1048576 && newBytes < 1073741824)
            {
                newBytes /= 1048576;
                byteType = "MB"; 
            }
            else
            {
                newBytes /= 1073741824;
                byteType = "GB";
            }

            if (decimalPlaces > 0)
                formatString += ":0.";

            for (int i = 0; i < decimalPlaces; i++)
                formatString += "0";

            formatString += "}";

            if (showByteTypes)
                formatString += byteType;

            return string.Format(formatString, newBytes);
        }


        private void webClient_DownloadFileCompleted(object s, AsyncCompletedEventArgs e)
        {
            
            if(e.Error != null)
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }

            if(e.Cancelled)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }

            else
            {
                lblProgress.Text = "Verifying Download....";
                progressBar.Style = ProgressBarStyle.Blocks;

                bgWorker.RunWorkerAsync(new string[] { this.tempFile });
            }
        }

        private void bgWorker_Woker(object s, DoWorkEventArgs e)
        {
            string file = ((string[])e.Argument)[0];
            e.Result = DialogResult.OK; 
        }

        private void bgWorker_Completed(object s, RunWorkerCompletedEventArgs e)
        {
            this.DialogResult = (DialogResult)e.Result;
            this.Close();
        }

        private void ApplicationUpdaterDownloadForm_FormClosed(object s, FormClosedEventArgs e)
        {
            if (webClient.IsBusy)
            {
                webClient.CancelAsync();
                this.DialogResult = DialogResult.Abort; 
            }

            if(bgWorker.IsBusy)
            {
                bgWorker.CancelAsync();
                this.DialogResult = DialogResult.Abort;
            }

        }

        public ApplicationUpdaterDownloadForm(Uri uri, Icon applicationIcon, string downloadPath)
        {
            InitializeComponent();

            if (applicationIcon != null)
                this.Icon = applicationIcon;

            this.tempFile = Path.GetTempFileName();
 
            webClient = new WebClient();

            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(webClient_DownloadFileCompleted);

            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_Woker);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_Completed); 

            try
            {
                webClient.DownloadFileAsync(uri, this.tempFile);
            }
            catch
            {
                this.DialogResult = DialogResult.No;
                this.Close(); 
            }


        }
    }
}
