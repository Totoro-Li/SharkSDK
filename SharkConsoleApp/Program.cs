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
            test.SetCredential("xxxxxxx", Utilities.StringToSecureString("xxxxxxxx"));
            test.IAAALogin();
            // Thread.Sleep(500000);
            Console.WriteLine("Time out, disposing");
        }
        
    }
}