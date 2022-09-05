using Newtonsoft.Json.Serialization;
using SharkSDK;

namespace SharkConsoleApp
{
    class Program
    {
        private static void Main()
        {
            Console.WriteLine($"GetSDKVersion={API.GetSDKVersion()}");
            BrowserHelper test = new();
            test.IAAALogin("xxxxxx","xxxxxx");
            Thread.Sleep(500000);
            Console.WriteLine("Time out, disposing");
        }
        
    }
}