using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace CSharpAndSolidWorks
{
    public static class MonitorInfo
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;

            [MarshalAs(UnmanagedType.U4)]
            public DisplayDeviceStateFlags StateFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        [Flags]
        public enum DisplayDeviceStateFlags
        {
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            PrimaryDevice = 0x4,
            MirroringDriver = 0x8,
            VGACompatible = 0x10,
            Removable = 0x20,
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }

        public static bool GetPhysicalSize(string deviceName, out int width, out int height)
        {
            width = height = 0;
            try
            {
                DISPLAY_DEVICE device;
                if (!GetDeviceByName(deviceName, out device))
                    return false;

                var edid = GetEdid(device);
                if (edid == null)
                    return false;

                width = ((edid[68] & 0xF0) << 4) + edid[66];
                height = ((edid[68] & 0x0F) << 8) + edid[67];
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static byte[] GetEdid(DISPLAY_DEVICE device)
        {
            RegistryKey key = null;
            try
            {
                var split = device.DeviceID.Split('\\');
                var monitor = split[1];
                var driver = string.Join("\\", split[2], split[3]);
                key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\Display\" + monitor);
                foreach (var sub in key.GetSubKeyNames())
                {
                    RegistryKey key2 = null;
                    RegistryKey key3 = null;
                    try
                    {
                        key2 = key.OpenSubKey(sub);
                        if (string.Compare(driver, key2.GetValue("Driver") as string, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            key3 = key2.OpenSubKey("Device Parameters");
                            return key3.GetValue("EDID") as byte[];
                        }
                    }
                    finally
                    {
                        if (key2 != null)
                            key2.Close();
                        if (key3 != null)
                            key3.Close();
                    }
                }
                return null;
            }
            finally
            {
                if (key != null)
                    key.Close();
            }
        }

        public static bool GetDeviceByName(string deviceName, out DISPLAY_DEVICE device)
        {
            var dd = new DISPLAY_DEVICE();
            dd.cb = Marshal.SizeOf(dd);
            uint id = 0;
            while (EnumDisplayDevices(null, id, ref dd, 0))
            {
                dd.cb = Marshal.SizeOf(dd);
                id++;
                if (!deviceName.StartsWith(dd.DeviceName))
                    continue;

                var ddMon = new DISPLAY_DEVICE();
                ddMon.cb = Marshal.SizeOf(ddMon);
                if (EnumDisplayDevices(dd.DeviceName, 0, ref ddMon, 0))
                {
                    device = ddMon;
                    return true;
                }
            }
            device = new DISPLAY_DEVICE();
            return false;
        }
    }
}