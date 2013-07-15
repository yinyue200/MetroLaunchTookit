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
    /*
    [ComImport, Guid ( "B63EA76D-1F85-456F-A19C-48159EFA858B" ), InterfaceType ( ComInterfaceType.InterfaceIsIUnknown )]
    interface IShellItemArray
    {
        // Not supported: IBindCtx
        [MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void BindToHandler ( [In, MarshalAs ( UnmanagedType.Interface )] IntPtr pbc, [In] ref Guid rbhid,
                     [In] ref Guid riid, out IntPtr ppvOut );

        [MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetPropertyStore ( [In] int Flags, [In] ref Guid riid, out IntPtr ppv );

        [MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetPropertyDescriptionList ( [In] ref NativeMethods.PROPERTYKEY keyType, [In] ref Guid riid, out IntPtr ppv );

        [MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetAttributes ( [In] NativeMethods.SIATTRIBFLAGS dwAttribFlags, [In] uint sfgaoMask, out uint psfgaoAttribs );

        [MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetCount ( out uint pdwNumItems );

        [MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetItemAt ( [In] uint dwIndex, [MarshalAs ( UnmanagedType.Interface )] out IShellItem ppsi );

        // Not supported: IEnumShellItems (will use GetCount and GetItemAt instead)
        [MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void EnumItems ( [MarshalAs ( UnmanagedType.Interface )] out IntPtr ppenumShellItems );
    }*/

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



    class Win32
    {
        public const int MAX_PATH = 260;
        public const long APPMODEL_ERROR_NO_APPLICATION = 15703;

        [DllImport("Kernel32.dll")]
        public static extern long GetApplicationUserModelId(
            [In] IntPtr hProcess,
            [In, Out] ref int applicationUserModelIdLength, // [In, Out] ref UInt32 applicationUserModelIdLength,
            [Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder applicationUserModelId); // [Out, MarshalAs(UnmanagedType.LPWStr)] String applicationUserModelId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(
            string lpClassName, 
            string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
