using System;
using System.Management.Automation;
using Microsoft.Windows.Appx.PackageManager.Commands;

namespace Win8Toolkit
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    class ConvertPackageToString : ArgumentTransformationAttribute
    {
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (inputData is PSObject)
            {
                var psInput = (PSObject)inputData;
                if (psInput.BaseObject is AppxPackage)
                {
                    var appxInput = (AppxPackage)psInput.BaseObject;
                    return appxInput.PackageFamilyName;
                }
            }

            return inputData;
        }
    }
}
