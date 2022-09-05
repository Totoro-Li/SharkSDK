namespace SharkSDK
{
    class ElectiveUrl
    {
        public const string Scheme = "https";
        public const string Host = "https://elective.pku.edu.cn/";
        public const string HomePage = "https://elective.pku.edu.cn/elective2008/";
        public const string SSOLoginRedirect = "http://elective.pku.edu.cn:80/elective2008/ssoLogin.do";
        public const string SSOLogin = "https://elective.pku.edu.cn/elective2008/ssoLogin.do";
        public const string Logout = "https://elective.pku.edu.cn/elective2008/logout.do";
        public const string HelpController = "https://elective.pku.edu.cn/elective2008/edu/pku/stu/elective/controller/help/HelpController.jpf";
        public const string ShowResults = "https://elective.pku.edu.cn/elective2008/edu/pku/stu/elective/controller/electiveWork/showResults.do";
        public const string SupplyCancel = "https://elective.pku.edu.cn/elective2008/edu/pku/stu/elective/controller/supplement/SupplyCancel.do";
        public const string Supplement = "https://elective.pku.edu.cn/elective2008/edu/pku/stu/elective/controller/supplement/supplement.jsp";
        public const string DrawServlet = "https://elective.pku.edu.cn/elective2008/DrawServlet";
        public const string Validate = "https://elective.pku.edu.cn/elective2008/edu/pku/stu/elective/controller/supplement/validate.do";
        public const string IAAALogin = "https://iaaa.pku.edu.cn/iaaa/oauth.jsp";
    }

    class TTShitu
    {
        public const string TTShituUrl = "http://api.ttshitu.com/predict";
    }

    class SdkDefinition
    {
        public static string SdkVersion = "1.0.0";
    }

    public enum SharkExitCodes
    {
        STATUS_OK = 0,
        GENERAL_ERROR = 1,
        CONNECTIVITY_ERROR = 2,
        CREDENTIAL_ERROR = 3,
    };

    class NotificationText
    {
        public const string WrongCredential = "用户名或密码错误";
        public const string AddOrDropLinkText = "补选退选";
        public const string SystemError = "系统异常";
    }

    class Timeouts
    {
        public const int MaxPageTimeout = 10;
        public const int MaxPartialPageTimeout = 5;
        public const int MaxAttempts = 3;
    }
}