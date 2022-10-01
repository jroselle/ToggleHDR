using System.Runtime.InteropServices;

public static partial class HDRToggle
{
    [StructLayout(LayoutKind.Sequential)]
    private struct POINTL
    {
        public int x;
        public int y;
    }
}