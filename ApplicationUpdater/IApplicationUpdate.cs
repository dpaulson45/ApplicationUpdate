using System;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms; 

namespace ApplicationUpdate
{
    public interface IApplicationUpdate
    {
        string ApplicationName { get; }
        string ApplicationID { get; }
        Assembly ApplicationAssembly { get; }
        Icon ApplicationIcon { get; }
        Uri ApplicationUpdaterXmlLocation { get; }
        Form ApplicationForm { get; }
    }
}
