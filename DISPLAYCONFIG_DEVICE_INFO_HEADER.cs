using System.Runtime.InteropServices;

public static partial class HDRToggle
{
    [StructLayout(LayoutKind.Sequential)]
    private struct DISPLAYCONFIG_DEVICE_INFO_HEADER
    {
        public DISPLAYCONFIG_DEVICE_INFO_TYPE type;
        public int size;
        public LUID adapterId;
        public uint id;
    }
}