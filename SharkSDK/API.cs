using RestSharp;

namespace SharkSDK;

public class API
{
    public static string GetSDKVersion()
    {
        return SdkDefinition.SdkVersion;
    }
}

public class TTShituRecognizer
{
    public static async Task Recognize()
    {
        RestClient client = new RestClient(TTShitu.TTShituUrl);
        client.Options.MaxTimeout = -1;
        var request = new RestRequest(TTShitu.TTShituUrl, Method.Post);
        request.AddParameter("username", "ses2014cc");
        request.AddParameter("password", "*****");
        request.AddParameter("image", "图片的base64(注意不含：data:image/jpg;base64,直接图片base64编码)");
        request.AddParameter("typeid", "3");
        //其他参数根据需要从 http 接口文档中添加
        RestResponse response = await client.ExecuteAsync(request);
        Console.WriteLine(response.Content);
    }
}