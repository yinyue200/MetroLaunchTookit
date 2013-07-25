using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Management.Deployment;

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
                // First, try to get all the apps from the AppxManifest.xml file. This seems to be more reliable
                // than digging through the registry.
                String path = package.InstalledLocation.Path + @"\AppxManifest.xml";

                XElement root = XDocument.Load(path).Root;
                XNamespace xmlns = root.GetDefaultNamespace();
                var xmlQuery = from node in root.Descendants(xmlns + "Application") select node;
                bool hasNodes = false;
                foreach (var node in xmlQuery)
                {
                    hasNodes = true;
                    XElement visualElements = node.Descendants(xmlns + "VisualElements").First();
                    WriteObject(new WindowsStoreApplication(
                        package.Id.FamilyName,
                        package.Id.FullName,
                        node.Attribute("Id").Value,
                        visualElements.Attribute("DisplayName").Value));
                }

                if (!hasNodes) {
                    try
                    {
                        RegistryKey packageKey = Registry.ClassesRoot.OpenSubKey(String.Format(@"ActivatableClasses\Package\{0}\Server", package.Id.FullName));
                        var regQuery = from subkey
                                       in packageKey.GetSubKeyNames()
                                       select packageKey.OpenSubKey(subkey).GetValue("AppUserModelId");
                        foreach (String value in regQuery)
                        {
                            // We assume that the given AppUserModelId is of the form PackageFamilyName + "!" + Id
                            // Double check that that's the case. We don't use Split in case the app has multiple exclamation
                            // points in the name
                            String familyName = package.Id.FamilyName;
                            int familyNameIndex = value.IndexOf(familyName + "!");
                            int familyNameLength = familyName.Length + 1;
                            if (familyNameIndex == 0 && value.Length > familyNameLength)
                            {
                                String id = value.Substring(familyNameLength, value.Length - familyNameLength);
                                WriteObject(new WindowsStoreApplication(
                                    familyName,
                                    package.Id.FullName,
                                    id,
                                    ""));
                            }
                        }
                    } 
                    catch (Exception)
                    {
                        // Usually a security violation. Just move on.
                    }
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
            InitializeInterfaces();

            PackageFamilyName = packageFamilyName;
            PackageFullName = packageFullName;
            Name = name;
            DisplayName = displayName;
            AppUserModelId = packageFamilyName + "!" + name;
        }

        private void InitializeInterfaces()
        {
            if (aam == null || pds == null) 
            {
                aam = (IApplicationActivationManager)new ApplicationActivationManager();
                pds = (IPackageDebugSettings)new PackageDebugSettings();
            }
        }

        private IntPtr GetWindowForApp()
        {
            IntPtr lastWindow = IntPtr.Zero;
            while ((lastWindow = Win32.FindWindowEx(IntPtr.Zero, lastWindow, "Windows.UI.Core.CoreWindow", IntPtr.Zero)) != IntPtr.Zero)
            {
                int pid = 1;
                if (Win32.GetWindowThreadProcessId(lastWindow, out pid) != 0)
                {
                    Process p = Process.GetProcessById(pid);
                    IntPtr handle;
                    try
                    {
                       handle = p.Handle;
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    StringBuilder sb = new StringBuilder(Win32.MAX_PATH);
                    int length = sb.Capacity;
                    long err = Win32.GetApplicationUserModelId(handle, ref length, sb);
                    if (err == 0 && sb.ToString() == AppUserModelId)
                    {
                        return lastWindow;
                    }
                }
            }
            return IntPtr.Zero;
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
