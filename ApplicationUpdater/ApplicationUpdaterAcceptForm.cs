using System;
using System.Windows.Forms;

namespace ApplicationUpdate
{
    internal partial class ApplicationUpdaterAcceptForm : Form
    {

        private IApplicationUpdate applicationUpdaterInfo;
        private ApplicationUpdaterXmlHandler applicationUpdaterXml;
        private ApplicationUpdaterDetailsForm applicationUpdaterDetailsForm; 
        


        internal ApplicationUpdaterAcceptForm(IApplicationUpdate applicationUpdaterInfo, ApplicationUpdaterXmlHandler applicationUpdaterXml)
        {
            InitializeComponent();

            this.applicationUpdaterInfo = applicationUpdaterInfo;
            this.applicationUpdaterXml = applicationUpdaterXml;

            this.Text = this.applicationUpdaterInfo.ApplicationName + " - Update Available";

            if (this.applicationUpdaterInfo.ApplicationIcon != null)
                this.Icon = this.applicationUpdaterInfo.ApplicationIcon;

            this.lblNewVersion.Text = string.Format("New Version: {0}", this.applicationUpdaterXml.Version.ToString()); 
            
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            if (this.applicationUpdaterDetailsForm == null)
                this.applicationUpdaterDetailsForm = new ApplicationUpdaterDetailsForm(this.applicationUpdaterInfo, this.applicationUpdaterXml);
            this.applicationUpdaterDetailsForm.ShowDialog(this); 
        }
    }
}
