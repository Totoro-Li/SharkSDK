using System.Net.Mime;
using OpenQA.Selenium.Support.Events;

namespace SharkSDK
{
    public abstract class ISharkListener
    {
        public delegate void OnLoginHandler();

        public OnLoginHandler? OnLogin;

        public delegate void OnCourseListReadyHandler(List<Course> courses);

        public OnCourseListReadyHandler? OnCourseListReady;

        public delegate void OnErrorHandler(SharkError error);

        public OnErrorHandler? OnError;
    }
    public static class Listeners
    {
        public static void DriverNavigated_Handler(object? sender, WebDriverNavigationEventArgs e)
        {
            Console.WriteLine("Page navigation completed");
        }

        public static void DriverException_Handler(object? sender, WebDriverExceptionEventArgs e)
        {
            Console.WriteLine("Driver fatal error");
            Environment.Exit(0);
        }
    }
}