using System.Runtime.InteropServices;

public static partial class HDRToggle
{
    [StructLayout(LayoutKind.Sequential)]
    private struct DISPLAYCONFIG_PATH_SOURCE_INFO
    {
        public LUID adapterId;
        public uint id;
        public uint modeInfoIdx;
        public DISPLAYCONFIG_SOURCE_FLAGS statusFlags;
    }
}