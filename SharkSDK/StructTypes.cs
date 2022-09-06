using System.Reflection;
using System.Runtime.InteropServices;
using OpenQA.Selenium;

namespace SharkSDK
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Course
    {
        public string CourseName { get; set; }

        public string CourseType { get; set; }

        public double Credits { get; set; }

        public double WeeklyHours { get; set; }

        public string Teacher { get; set; }

        public int Classid { get; set; }

        public string HostDept { get; set; }

        public string? Grade { get; set; } //Nullable

        public string? VenueExamInfo { get; set; } //Nullable

        public (int, int) Quota { get; set; }
    }
}