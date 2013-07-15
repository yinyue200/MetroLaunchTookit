using System;
using System.Management.Automation;

namespace Win8Toolkit
{
    /// <summary>
    /// Tells if Start screen or any Windows Store apps are visible on any monitor
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AppOrStartVisibility")]
    public class AppOrStartVisibility: PSCmdlet
    {
        IAppVisibility av = (IAppVisibility)new AppVisibility();

        protected override void ProcessRecord()
        {
            bool startOrAppsVisible = false;
            av.IsLauncherVisible(out startOrAppsVisible);

            if (!startOrAppsVisible)
            {
                Win32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, new EnumMonitorsDelegate(
                    (IntPtr hMonitor, IntPtr hdcMonitor, ref RectStruct lprcMonitor, IntPtr dwData) =>
                        {
                            MONITOR_APP_VISIBILITY mav = MONITOR_APP_VISIBILITY.MAV_UNKNOWN;
                            av.GetAppVisibilityOnMonitor(hMonitor, out mav);
                            if (mav == MONITOR_APP_VISIBILITY.MAV_APP_VISIBLE)
                            {
                                startOrAppsVisible = true;
                            }
                            return !startOrAppsVisible;
                        }), IntPtr.Zero);
            }

            WriteObject(startOrAppsVisible);
            base.ProcessRecord();
        }
    }
}
