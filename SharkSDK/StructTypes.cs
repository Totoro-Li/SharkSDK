using System.Reflection;
using System.Runtime.InteropServices;
using OpenQA.Selenium;

namespace SharkSDK;

public class StructTypes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Course
    {
        public string CourseName { get; set; }

        public string CourseType { get; set; }

        public double Credits { get; set; }

        public double WeeklyHours { get; set; }

        public string faculty { get; set; }

        public int classid { get; set; }

        public string HostDept { get; set; }

        public string? grade { get; set; } //Nullable

        public string? VenueExamInfo { get; set; } //Nullable

        public (int, int) Quota { get; set; }
    }
}