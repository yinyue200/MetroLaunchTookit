using System;
using System.Linq;
using System.Management.Automation;
using System.Xml.Linq;
using Windows.Management.Deployment;
using System.Collections.Generic;
using Windows.ApplicationModel;

namespace Win8Toolkit
{
    [Cmdlet(VerbsCommon.Get, "AppsFromAppxPackage")]
    public class AppEnumerate : PSCmdlet
    {
        private PackageManager pm = new PackageManager();
        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        [ConvertPackageToString]
        public String PackageFullName { get; set; }
 
        protected override void ProcessRecord()
        {
            IEnumerable<Package> packages;
            if (this.PackageFullName == null)
            {
                packages = pm.FindPackagesForUser("");
            }
            else
            {
                packages = pm.FindPackagesForUser("", this.PackageFullName);
            }
            foreach (var package in packages)
            {
                String path = package.InstalledLocation.Path + @"\AppxManifest.xml";

                XElement root = XDocument.Load(path).Root;
                XNamespace xmlns = root.GetDefaultNamespace();
                var q = from node in root.Descendants(xmlns + "Application") select node;
                foreach (var node in q)
                {
                    XElement visualElements = node.Descendants(xmlns + "VisualElements").First();
                    WriteObject(new WindowsStoreApplication(
                        node.Attribute("Id").Value,
                        package.Id.FullName,
                        visualElements.Attribute("DisplayName").Value));
                }
            }
        }
    }

    public class WindowsStoreApplication
    {
        public WindowsStoreApplication(
            String packageName,
            String name,
            String displayName)
        {
            _packageName = packageName;
            _name = name;
            _displayName = displayName;
        }

        private String _packageName;
        public String PackageName { get { return _packageName; } }
        private String _name;
        public String Name { get { return _name; } }
        private String _displayName;
        public String DisplayName { get { return _displayName; } }


    }
}
