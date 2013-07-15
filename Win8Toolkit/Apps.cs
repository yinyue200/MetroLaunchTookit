using System;
using System.Linq;
using System.Management.Automation;
using System.Xml.Linq;
using Windows.Management.Deployment;
using System.Collections.Generic;
using Windows.ApplicationModel;

namespace Win8Toolkit
{
    [Cmdlet(VerbsCommon.Get, "WindowsStoreApps")]
    public class AppEnumerate : PSCmdlet
    {
        private PackageManager pm = new PackageManager();

        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        [ConvertPackageToString]
        public String PackageFamilyName { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public String User { get; set; }

        [Parameter(Mandatory = false)]
        public bool AllUsers { get; set; }
 
        protected override void ProcessRecord()
        {
            IEnumerable<Package> packages;
            if (AllUsers)
            {
                if (this.PackageFamilyName == null)
                {
                    packages = pm.FindPackages();
                }
                else
                {
                    packages = pm.FindPackages(this.PackageFamilyName);
                }
            }
            else
            {
                if (this.PackageFamilyName == null)
                {
                    packages = pm.FindPackagesForUser(User ?? "");
                }
                else
                {
                    packages = pm.FindPackagesForUser(User ?? "", this.PackageFamilyName);
                }
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
                        package.Id.FamilyName,
                        package.Id.FullName,
                        node.Attribute("Id").Value,
                        visualElements.Attribute("DisplayName").Value));
                }
            }
        }
    }

    public class WindowsStoreApplication
    {
        private static IApplicationActivationManager aam;
        private static IPackageDebugSettings pds;

        public WindowsStoreApplication(
            String packageFamilyName,
            String packageFullName,
            String name,
            String displayName)
        {
            PackageFamilyName = packageFamilyName;
            PackageFullName = packageFullName;
            Name = name;
            DisplayName = displayName;
            AppUserModelId = packageFamilyName + "!" + name;

            // Note that this is not thread safe
            aam = (IApplicationActivationManager)new ApplicationActivationManager();
            pds = (IPackageDebugSettings)new PackageDebugSettings();
        }

        private IntPtr GetWindowForApp()
        {
            return Win32.FindWindow("Windows.UI.Core.CoreWindow", DisplayName);
        }

        public String PackageFullName { get; private set; }
        public String PackageFamilyName { get; private set; }
        public String Name { get; private set; }
        public String DisplayName { get; private set; }
        public String AppUserModelId { get; private set; }

        /// <summary>
        /// Launches an application using activation, providing blank arguments
        /// <returns>Process ID of launched app</returns>
        /// </summary>
        public UInt32 Launch()
        {
            return Launch("");
        }

        /// <summary>
        /// Launches an application using activation and the specified arguments
        /// <param name="arguments">Arguments to pass to the app</param>
        /// <returns>Process ID of launched app</returns>
        /// </summary>
        public UInt32 Launch(String arguments)
        {
            UInt32 pid = 0;
            aam.ActivateApplication(AppUserModelId, arguments, ACTIVATEOPTIONS.AO_NONE, out pid);
            return pid;
        }

        /// <summary>
        /// Tells if the application has some UI running
        /// </summary>
        public PackageExecutionState ExecutionState
        {
            get
            {
                PackageExecutionState pes;
                pds.GetPackageExecutionState(PackageFullName, out pes);

                // PackageDebugSettings gives the state for the whole package. It's possible another app in
                // the package is running, so check if there's a window for the app
                if ((pes != PackageExecutionState.Terminated || pes != PackageExecutionState.Unknown) && GetWindowForApp() == IntPtr.Zero)
                {
                    pes = PackageExecutionState.Terminated;
                }

                return pes;
            }
        }

        /// <summary>
        ///  Tell the system to suspend the app
        /// </summary>
        public void Suspend()
        {
            pds.Suspend(PackageFullName);
        }

        public void Resume()
        {
            pds.Resume(PackageFullName);
        }

        /// <summary>
        ///  Tell the system to terminate this app and all apps in the package
        /// </summary>
        public void Terminate()
        {
            pds.TerminateAllProcesses(PackageFullName);
        }

        /// <summary>
        /// Attempts to switch to the app if it is running
        /// </summary>
        /// <returns>Tells if switching to the app was successful</returns>
        public bool SwitchTo()
        {
            IntPtr hwnd = GetWindowForApp();
            if (hwnd != IntPtr.Zero)
            {
                return Win32.SetForegroundWindow(hwnd);
            }
            return false;
        }
    }
}
