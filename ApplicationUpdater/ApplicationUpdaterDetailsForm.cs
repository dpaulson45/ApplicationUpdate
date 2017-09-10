using System;
using System.Windows.Forms;

namespace ApplicationUpdate
{
    internal partial class ApplicationUpdaterDetailsForm : Form
    {
        internal ApplicationUpdaterDetailsForm(IApplicationUpdate applicationUpdaterInfo, ApplicationUpdaterXmlHandler applicationUpdaterXml)
        {
            InitializeComponent();

            if (applicationUpdaterInfo.ApplicationIcon != null)
                this.Icon = applicationUpdaterInfo.ApplicationIcon;

            this.Text = applicationUpdaterInfo.ApplicationName + " - Update Info";
            this.lblVersions.Text = string.Format("Current Version: {0}\nUpdate Version: {1}", applicationUpdaterInfo.ApplicationAssembly.GetName().Version.ToString(),
                applicationUpdaterXml.Version.ToString());
            this.txtDescription.Text = applicationUpdaterXml.Description; 
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Control && e.KeyCode == Keys.C))
                e.SuppressKeyPress = true; 
        }
    }
}
