using System;
using System.ComponentModel;
using System.Runtime.InteropServices;


public static partial class HDRToggle
{
    [DllImport("user32")]
    private static extern int GetDisplayConfigBufferSizes(QDC flags, out int numPathArrayElements, out int numModeInfoArrayElements);

    [DllImport("user32")]
    private static extern int QueryDisplayConfig(QDC flags, ref int numPathArrayElements, [In, Out] DISPLAYCONFIG_PATH_INFO[] pathArray, ref int numModeInfoArrayElements, [In, Out] DISPLAYCONFIG_MODE_INFO[] modeInfoArray, IntPtr currentTopologyId);

    [DllImport("user32")]
    private static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO requestPacket);


    [DllImport("user32")]
    private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

    /// <summary>
    /// Sends the windows (11?) hotkey to enable/disable HDR
    /// Would prefer a pinvoke method, just haven't found one
    /// </summary>
    static void ToggleHDR()
    {
        Console.WriteLine("Toggling HDR using Windows 11 hotkey");

        int KEYEVENTF_KEYDOWN = 0x0000;
        int KEYEVENTF_KEYUP = 0x0002;

        byte VK_LMENU = 0xA4;
        byte VK_LWIN = 0x5B;
        byte B = 0x42; // B

        keybd_event(VK_LWIN, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_LMENU, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(B, 0, KEYEVENTF_KEYDOWN, 0);

        keybd_event(B, 0, KEYEVENTF_KEYUP, 0);
        keybd_event(VK_LMENU, 0, KEYEVENTF_KEYUP, 0);
        keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, 0);
    }


    // Check to see if HDR is enabled, if not enable it.
    public static void Main(string[] args)
    {
        bool hdrEnabled = IsHdrEnabled();

        Console.WriteLine($"HDR is {(hdrEnabled ? "Enabled" : "Disabled")}");

        if (args.Any()) {
            string first = args.First();

            if (bool.TryParse(first, out bool shouldEnable) && shouldEnable != hdrEnabled)
            {
                ToggleHDR();
            } else {
                Console.WriteLine("Can't parse param, try true to enable or false to disable HDR.");
            }
        }

    }

    // Inspiration:  (Thank you kind person!) https://stackoverflow.com/a/66160049
    // returns true if HDR is enabled on all devices
    // otherwise false    
    private static bool IsHdrEnabled()
    {
        bool IsHdrEnabled = true;

        int err = GetDisplayConfigBufferSizes(QDC.QDC_ONLY_ACTIVE_PATHS, out int pathCount, out int modeCount);
        if (err != 0)
            throw new Win32Exception(err);

        var paths = new DISPLAYCONFIG_PATH_INFO[pathCount];
        var modes = new DISPLAYCONFIG_MODE_INFO[modeCount];
        err = QueryDisplayConfig(QDC.QDC_ONLY_ACTIVE_PATHS, ref pathCount, paths, ref modeCount, modes, IntPtr.Zero);
        if (err != 0)
            throw new Win32Exception(err);

        foreach (DISPLAYCONFIG_PATH_INFO path in paths)
        {
            var colorInfo = new DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO();
            colorInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;
            colorInfo.header.size = Marshal.SizeOf<DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO>();
            colorInfo.header.adapterId = path.targetInfo.adapterId;
            colorInfo.header.id = path.targetInfo.id;
            err = DisplayConfigGetDeviceInfo(ref colorInfo);
            if (err != 0)
                throw new Win32Exception(err);

            IsHdrEnabled = IsHdrEnabled && colorInfo.AdvancedColorEnabled;
        }

        return IsHdrEnabled;
    }

}