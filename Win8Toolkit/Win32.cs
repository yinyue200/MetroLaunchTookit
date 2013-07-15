using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Win8Toolkit
{
    [ComImport, Guid("45BA127D-10A8-46EA-8AB7-56EA9078943C")]
    class ApplicationActivationManager
    {
    }

    enum ACTIVATEOPTIONS
    {
        AO_NONE           = 0x00000000,  // No flags set
        AO_DESIGNMODE     = 0x00000001,  // The application is being activated for design mode, and thus will not be able to
                                         // to create an immersive window. Window creation must be done by design tools which
                                         // load the necessary components by communicating with a designer-specified service on
                                         // the site chain established on the activation manager.  The splash screen normally
                                         // shown when an application is activated will also not appear.  Most activations
                                         // will not use this flag.
        AO_NOERRORUI      = 0x00000002,  // Do not show an error dialog if the app fails to activate.                                
        AO_NOSPLASHSCREEN = 0x00000004,  // Do not show the splash screen when activating the app.
    }

    [ComImport, Guid("2e941141-7f97-4756-ba1d-9decde894a3d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IApplicationActivationManager
    {
        // Activates the specified immersive application for the "Launch" contract, passing the provided arguments
        // string into the application.  Callers can obtain the process Id of the application instance fulfilling this contract.
        UInt32 ActivateApplication(
            [In, MarshalAs(UnmanagedType.LPWStr)] String appUserModelId,
            [In, MarshalAs(UnmanagedType.LPWStr)] String arguments,
            [In] ACTIVATEOPTIONS options,
            [Out] out UInt32 processId);
        // This is not supported right now
        UInt32 ActivateForFile( // This is not supported right now
            [In, MarshalAs(UnmanagedType.LPWStr)] String appUserModelId,
            [In] IntPtr itemArray, //[In] IShellItemArray itemArray,
            [In, MarshalAs(UnmanagedType.LPWStr)] String verb,
            [Out] UInt32 processId);
        // This is not supported right now
        UInt32 ActivateForProtocol(
            [In, MarshalAs(UnmanagedType.LPWStr)] String appUserModelId,
            [In] IntPtr itemArray, //[In] IShellItemArray itemArray,
            [Out] UInt32 processId);
    }


    [ComImport, Guid("B1AEC16F-2383-4852-B0E9-8F0B1DC66B4D")]
    class PackageDebugSettings
    {
    }

    /*
    Renamed from Shobjidl.h:     

    enum PACKAGE_EXECUTION_STATE
    {
        PES_UNKNOWN	= 0,
        PES_RUNNING	= 1,
        PES_SUSPENDING	= 2,
        PES_SUSPENDED	= 3,
        PES_TERMINATED	= 4
    }
    */
    public enum PackageExecutionState
    {
        Unknown = 0,
        Running = 1,
        Suspending = 2,
        Suspended = 3,
        Terminated = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    struct GUID
    {
       public int a;
       public short b;
       public short c;
       [MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]
       public byte[] d;
    }

    [ComImport, Guid("F27C3930-8029-4AD1-94E3-3DBA417810C1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IPackageDebugSettings
    {
        void EnableDebugging(
            [In] String packageFullName, 
            [In, Optional] String debuggerCommandLine, 
            [In, Optional] String[] environment);
        void DisableDebugging([In] String packageFullName);
        void Suspend([In] String packageFullName);
        void Resume([In] String packageFullName);
        void TerminateAllProcesses([In] String packageFullName);
        void SetTargetSessionId([In] UIntPtr sessionId);
        void EnumerateBackgroundTasks(
            [In] String packageFullName, 
            [Out] out UIntPtr taskCount, 
            [Out] out GUID[] taskIds,
            [Out] out String[] taskNames
            );

        void ActivateBackgroundTask([In] ref GUID taskId);
        void StartServicing([In] String packageFullName);
        void StopServicing([In] String packageFullName);
        void StartSessionRedirection([In] String packageFullName, [In] UIntPtr sessionId);
        void StopSessionRedirection([In] String  packageFullName);
        void GetPackageExecutionState([In] String packageFullName, [Out] out PackageExecutionState packageExecutionState);

        void RegisterForPackageStateChanges([In] String packageFullName, [In] IntPtr pPackageExecutionStateChangeNotification, [Out] out UInt32 pdwCookie);
        void UnregisterForPackageStateChanges([In] UInt32 dwCookie);
    }


    [ComImport(), Guid("7E5FE3D9-985F-4908-91F9-EE19F9FD1514")]
    class AppVisibility
    {
    }

    enum MONITOR_APP_VISIBILITY
    {
       MAV_UNKNOWN = 0,         // The mode for the monitor is unknown
       MAV_NO_APP_VISIBLE = 1,
       MAV_APP_VISIBLE = 2
    };

    [ComImport(), Guid("6584CE6B-7D82-49C2-89C9-C6BC02BA8C38"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IAppVisibilityEvents
    {
       void AppVisibilityOnMonitorChanged([In] IntPtr hMonitor,
                                  [In] MONITOR_APP_VISIBILITY previousMode,
                                  [In] MONITOR_APP_VISIBILITY currentMode);

       void LauncherVisibilityChange([In] bool currentVisibleState);
    }

    [ComImport(), Guid("2246EA2D-CAEA-4444-A3C4-6DE827E44313"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IAppVisibility
    {
       void GetAppVisibilityOnMonitor([In] IntPtr hMonitor, [Out] out MONITOR_APP_VISIBILITY pMode);
       void IsLauncherVisible([Out] out bool pfVisible);
       void Advise([In] IAppVisibilityEvents pCallback, [Out] out UInt32 pdwCookie);
       void Unadvise([In] UInt32 dwCookie);
    }

    internal delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RectStruct lprcMonitor, IntPtr dwData);

    [StructLayout(LayoutKind.Sequential)]
    public struct RectStruct
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    class Win32
    {
        public const int MAX_PATH = 260;
        public const long APPMODEL_ERROR_NO_APPLICATION = 15703;

        [DllImport("Kernel32.dll")]
        public static extern long GetApplicationUserModelId(
            [In] IntPtr hProcess,
            [In, Out] ref int applicationUserModelIdLength,
            [Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder applicationUserModelId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(
            string lpClassName, 
            string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, IntPtr lpszWindow);

        [DllImport("user32.dll", SetLastError=true)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);
        
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
