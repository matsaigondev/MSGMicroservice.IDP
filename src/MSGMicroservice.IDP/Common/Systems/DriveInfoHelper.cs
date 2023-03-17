using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MSGMicroservice.IDP.Common.Systems
{
    public class DriveInfoHelper
    {
        public static void CheckDriveInfo()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            decimal s1 = 0;
            foreach (DriveInfo d in allDrives)
            {
                Console.WriteLine("Drive {0}", d.Name);
                Console.WriteLine("  Drive type: {0}", d.DriveType);
                if (d.IsReady == true)
                {
                    Console.WriteLine("  Volume label: {0}", d.VolumeLabel);
                    Console.WriteLine("  File system: {0}", d.DriveFormat);
                    Console.WriteLine("  Available space to current user:{0, 15} GB",
                        d.AvailableFreeSpace / 1024 / 1024);

                    Console.WriteLine("  Total available space:          {0, 15} GB", d.TotalFreeSpace / 1024 / 1024);

                    Console.WriteLine("  Total size of drive:            {0, 15} GB ", d.TotalSize / 1024 / 1024);
                    s1 += d.AvailableFreeSpace / 1024 / 1024;
                }
            }

            decimal s = 0;
            foreach (var item in allDrives)
            {
                Console.WriteLine("Drive: {0} TotalSpace:{1} FreeSpace:{2}", item.Name, item.TotalSize / 1024 / 1024,
                    item.AvailableFreeSpace / 1024 / 1024);
                s += item.AvailableFreeSpace / 1024 / 1024;
            }

            Console.WriteLine("Drive free now: {0} {1}", s, s1);
        }

        // public static string GetDiskSpace()
        // {
        //     string diskSpace = string.Empty;
        //     if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        //     {
        //         diskSpace = GetDiskSpaceInMac();
        //     }
        //
        //     if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        //     {
        //         diskSpace = GetDiskSpaceInLinux();
        //     }
        //
        //     if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        //     {
        //         diskSpace = GetDiskSpaceInWindows();
        //     }
        //
        //     return diskSpace;
        // }
    }
}