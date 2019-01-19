using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win8Toolkit;
using Windows.ApplicationModel;
using Windows.Management.Deployment;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            PackageManager pm = new PackageManager();
            var pkg = pm.FindPackage("Microsoft.ScreenSketch_10.1811.3471.0_x64__8wekyb3d8bbwe");
            var info=AppEnumerate.GetIdFromPackage(pkg);
            var app = new WindowsStoreApplication(info.FamilyName, pkg.Id.FullName, info.Id, info.DisplayName);
            app.LaunchForFile(@"D:\yinyu\Pictures\6d81800a19d8bc3e2f91300f8e8ba61ea9d3455d.jpg");

        }
    }
}
