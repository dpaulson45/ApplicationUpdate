using System;
using System.Net;
using System.Xml;

namespace ApplicationUpdate
{
    internal class ApplicationUpdaterXmlHandler
    {
        //private variables 
        private Version appVersion;
        private Uri appUri;
        private string appFilename;
        private string appDescription;
        private string appLaunchArgs; 

        internal Version Version
        {
            get { return this.appVersion; }
        }

        internal Uri Uri
        {
            get { return this.appUri; }
        }

        internal string Filename
        {
            get { return this.appFilename; }
        }

        internal string Description
        {
            get { return this.appDescription; }
        }

        internal string LaunchArgs
        {
            get { return this.appLaunchArgs; }
        }

        internal ApplicationUpdaterXmlHandler(Version version, Uri uri, string filename, string description, string launchArgs)
        {
            this.appVersion = version;
            this.appUri = uri;
            this.appFilename = filename;
            this.appDescription = description;
            this.appLaunchArgs = launchArgs; 
        }

        internal bool AppIsNewer (Version version)
        {
            return this.appVersion > version;
        }

        internal static bool UriExists(Uri checkLocation)
        {
            try
            {
                //Attempt the request 
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(checkLocation);
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                //we want to make sure that we close the request 
                webResponse.Close();

                return webResponse.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                //doesn't exists at this location return false 
                return false; 
            }
        }

        internal static ApplicationUpdaterXmlHandler ParseXmlNode (Uri location, string appName)
        {
            Version version = null;
            string url = "", filename = "", description = "", launchArgs = ""; 

            try
            {
                //create and load the document 
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(location.AbsoluteUri);

                //create xml node to store data 
                XmlNode xmlNode = xmlDoc.DocumentElement.SelectSingleNode("//update[@appId='" + appName + "']");

                if (xmlNode == null) { return null; }

                //get the data from the node object 
                version = Version.Parse(xmlNode["version"].InnerText);
                url = xmlNode["url"].InnerText;
                filename = xmlNode["fileName"].InnerText;
                description = xmlNode["description"].InnerText;
                launchArgs = xmlNode["launchArgs"].InnerText;

                return new ApplicationUpdaterXmlHandler(version, new Uri(url), filename, description, launchArgs);
            }
            catch
            {
                return null;
            }
        }

    }
}
