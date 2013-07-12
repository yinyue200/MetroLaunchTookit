using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Win8Toolkit
{
    [ComImport(), Guid("45BA127D-10A8-46EA-8AB7-56EA9078943C")]
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
            [Out] UInt32 processId);
        UInt32 ActivateForFile(
            [In, MarshalAs(UnmanagedType.LPWStr)] String appUserModelId,
            [In] IntPtr itemArray, //[In] IShellItemArray itemArray,
            [In, MarshalAs(UnmanagedType.LPWStr)] String verb,
            [Out] UInt32 processId);
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

    class Win32
    {
    }
}
