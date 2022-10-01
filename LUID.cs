using System.Runtime.InteropServices;

public static partial class HDRToggle
{
    [StructLayout(LayoutKind.Sequential)]
    private struct LUID
    {
        public uint LowPart;
        public int HighPart;

        public long Value => ((long)HighPart << 32) | LowPart;
        public override string ToString() => Value.ToString();
    }
}