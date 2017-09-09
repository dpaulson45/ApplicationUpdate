using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms; 

namespace ApplicationUpdate
{
    public class ApplicationUpdater
    {
        private IApplicationUpdate applicationUpdaterInfo;
        private BackgroundWorker bgWorker;
        private BackgroundWorker bgWorkerCheckUpdate;
        private bool newUpdate = false;

        public ApplicationUpdater(IApplicationUpdate applicationUpdaterInfo)
        {
            this.applicationUpdaterInfo = applicationUpdaterInfo;

            bgWorker = new BackgroundWorker();
            bgWorkerCheckUpdate = new BackgroundWorker();
            bgWorkerCheckUpdate.DoWork += bgWorkerCheckUpdate_DoWork;
            bgWorkerCheckUpdate.RunWorkerCompleted += bgWorkerCheckUpdate_RunWorkerCompleted;
            if (!bgWorkerCheckUpdate.IsBusy)
                bgWorkerCheckUpdate.RunWorkerAsync(this.applicationUpdaterInfo);
            bgWorker.DoWork += bgWorker_DoWork;
            bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted; 
        }

        public bool NewUpdate()
        {
            return newUpdate;
        }

        public void DoUpdate()
        {
            if (!this.bgWorker.IsBusy)
                this.bgWorker.RunWorkerAsync(this.applicationUpdaterInfo); 
        }

        private void bgWorkerCheckUpdate_DoWork(object s, DoWorkEventArgs e)
        {
            IApplicationUpdate application = (IApplicationUpdate)e.Argument;
            if (!ApplicationUpdaterXmlHandler.UriExists(application.ApplicationUpdaterXmlLocation))
                e.Cancel = true;
            else
                e.Result = ApplicationUpdaterXmlHandler.ParseXmlNode(application.ApplicationUpdaterXmlLocation, application.ApplicationID); 
        }

        private void bgWorkerCheckUpdate_RunWorkerCompleted(object s, RunWorkerCompletedEventArgs e)
        {
            if(!e.Cancelled)
            {
                ApplicationUpdaterXmlHandler checker = (ApplicationUpdaterXmlHandler)e.Result;
                if (checker != null && checker.AppIsNewer(this.applicationUpdaterInfo.ApplicationAssembly.GetName().Version))
                    newUpdate = true;
                else
                    newUpdate = false; 
            }
        }

        private void bgWorker_DoWork(object s, DoWorkEventArgs e)
        {
            IApplicationUpdate application = (IApplicationUpdate)e.Argument;

            if (!ApplicationUpdaterXmlHandler.UriExists(application.ApplicationUpdaterXmlLocation))
                e.Cancel = true;
            else
                e.Result = ApplicationUpdaterXmlHandler.ParseXmlNode(application.ApplicationUpdaterXmlLocation, application.ApplicationID); 
        }

        private void bgWorker_RunWorkerCompleted(object s, RunWorkerCompletedEventArgs e)
        {
            if(!e.Cancelled)
            {
                ApplicationUpdaterXmlHandler xmlUpdater = (ApplicationUpdaterXmlHandler)e.Result; 

                if(xmlUpdater != null && xmlUpdater.AppIsNewer(this.applicationUpdaterInfo.ApplicationAssembly.GetName().Version))
                {
                    if (new ApplicationUpdaterAcceptForm(this.applicationUpdaterInfo, xmlUpdater).ShowDialog(this.applicationUpdaterInfo.ApplicationForm) == DialogResult.Yes)
                        this.DownloadUpdate(xmlUpdater); 
                }

            }
        }

        private void DownloadUpdate(ApplicationUpdaterXmlHandler applicationUpdaterXml)
        {
            ApplicationUpdaterDownloadForm downloadForm = new ApplicationUpdaterDownloadForm(applicationUpdaterXml.Uri, this.applicationUpdaterInfo.ApplicationIcon, this.applicationUpdaterInfo.ApplicationAssembly.Location);
            DialogResult diaResults = downloadForm.ShowDialog(this.applicationUpdaterInfo.ApplicationForm); 

            switch(diaResults)
            {
                case DialogResult.OK:
                    string currentPath = this.applicationUpdaterInfo.ApplicationAssembly.Location;
                    string newPath = Path.GetDirectoryName(currentPath) + "\\" + applicationUpdaterXml.Filename;

                    UpdateApplication(downloadForm.TempFilePath, currentPath, newPath, applicationUpdaterXml.LaunchArgs);

                    Application.Exit(); 

                    break;

                case DialogResult.Abort:
                    MessageBox.Show("The update download was cancelled.\nThis program has not been modified.", "Update Download Cancalled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break; 
                
                default:
                    MessageBox.Show("There was a problem downloading the update. \n Please try again later", "Update Download Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break; 
            }

        }

        private void UpdateApplication(string tmpFilePath, string currentPath, string newPath, string launchArgs)
        {
            //string argument = "/C Choice /C Y /N /D Y /T 4 & START \"\" /D \"{0}\" \"{1}\" {2}";
            //"/evn /user:" + "Administrator" + 
            string argument = "/C Choice /C Y /N /D Y /T 4 & Choice /C Y /N /D Y /T 2 & Move /Y \"{0}\" \"{1}\" & Start \"\" /D \"{2}\" \"{3}\" {4}";
            //string argumentMove = "/C Choice /C Y /N /D Y /T 4 & Del /F /Q \"{0}\" & Choice /C Y /N /D Y /T 2 & Move /Y \"{1}\" \"{2}\"";
            //string argumentInstall = "Start \"\" /D \"{0}\" \"{1}\" {2}";

            ProcessStartInfo psi = new ProcessStartInfo();

            psi.Arguments = string.Format(argument, tmpFilePath, newPath, Path.GetDirectoryName(newPath), Path.GetFileName(newPath), launchArgs);
            psi.UseShellExecute = true;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Normal;
            psi.Verb = "runas";
            psi.FileName = "cmd.exe"; 

            try
            {
                Process.Start(psi);
            }
            catch
            {
                MessageBox.Show("There was a problem with trying to run the setup of the update.\nPlease Run As Administrator of this program", "Setup Install Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

    }
}
