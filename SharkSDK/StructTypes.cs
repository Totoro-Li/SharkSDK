using System.Runtime.InteropServices;

namespace SharkSDK;

public class StructTypes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Course
    {
        public string CourseName;

        public int classid;

        public double credits;

        public int MaxQuota;

        public int UsedQuota;
    }
}