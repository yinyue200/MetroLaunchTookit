using System;
using System.Management.Automation;
using Microsoft.Windows.Appx.PackageManager.Commands;
using Windows.Management.Deployment;
using Windows.ApplicationModel;

namespace Win8Toolkit
{
    [Cmdlet(VerbsCommon.Get, "AppsFromAppxPackage")]
    public class AppEnumerate : Cmdlet
    {
        private PackageManager pm = new PackageManager();
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        [ConvertPackageToString]
        public String PackageFullName { get; set; }
 
        protected override void ProcessRecord()
        {
            Package package = pm.FindPackage(this.PackageFullName);
            String path = package.InstalledLocation + @"\AppxManifest.xml";
            using (
        }
    }
}
