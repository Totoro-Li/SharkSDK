using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;

namespace SharkSDK;

public class BrowserHelper : Base
{
    private ICollection<Cookie>? SessionCookie { get; set; }

    public delegate void OnLoadResp();

    public OnLoadResp? OnLogin;

    public OnLoadResp? OnTableReady;

    public TableManip tableManip { get; set; }

    private WebDriverWait MaxPageTimeoutWait { get; set; }

    WebDriverWait MaxPartialPageTimeoutWait { get; set; }

    public delegate void OnCourseAvailableResp();

    public int TotalPages { get; set; } = 1;


    private ObservableCollection<Thread> _threadPool = new();

    public BrowserHelper()
    {
        var options = new ChromeOptions();

        #region Driver Options

        //options.AddArguments("-headless");
        options.AddArgument("disable-infobars");
        options.AddExcludedArgument("enable-automation");
        options.AddAdditionalOption("useAutomationExtension", false);
        options.AddUserProfilePreference("credentials_enable_service", false);
        options.AddUserProfilePreference("profile.password_manager_enabled", false);

        #endregion

        SetDriver(new ChromeDriver(options));

        MaxPageTimeoutWait = new(Driver, TimeSpan.FromSeconds(Timeouts.MaxPageTimeout));
        MaxPartialPageTimeoutWait = new(Driver, TimeSpan.FromSeconds(Timeouts.MaxPartialPageTimeout));
        tableManip = new();
        OnLogin = OnSuccessfulLoginImpl;
        OnTableReady = OnTableReadyImpl;
    }

    public void OnSuccessfulLoginImpl()
    {
        if (Driver == null) throw new SharkError((int)SharkExitCodes.DRIVER_ERROR);
        SharkExitCodes status = SharkExitCodes.GENERAL_ERROR;
        SessionCookie = Driver.Manage().Cookies.AllCookies;
        for (int attempt = 0; attempt < Timeouts.MaxAttempts; ++attempt)
        {
            Driver.Navigate().Refresh();
            MaxPageTimeoutWait.Until((x) => ((IJavaScriptExecutor)Driver).ExecuteScript("return document.readyState").Equals("complete"));
            try
            {
                Driver.FindElement(By.LinkText(NotificationText.AddOrDropLinkText)).Click();
            }
            catch (NoSuchElementException)
            {
                continue;
            }

            if (Driver.Title == NotificationText.SystemError)
            {
                Driver.Navigate().Back();
                continue;
            }

            if (Driver.Title == NotificationText.AddOrDropLinkText)
            {
                status = SharkExitCodes.STATUS_OK;
                break;
            }
        }

        if (status == SharkExitCodes.STATUS_OK)
            OnTableReady?.Invoke();
    }


    public void OnTableReadyImpl()
    {
        if (Driver == null) throw new SharkError((int)SharkExitCodes.DRIVER_ERROR);
        IWebElement operationZone = Driver.FindElement(By.XPath(PageXPath.SchemePageOperationZone));
        var pageText = operationZone.Text; //e.g. Page 1 of 3
        TotalPages = int.Parse(pageText.Split(' ')[3]);
        for (int i = 1; i <= TotalPages; ++i)
        {
            /*
            Thread t = new Thread(new ParameterizedThreadStart(ParallelReadCourseTable));
            _threadPool.Add(t);
            t.Start(i);
            */
            ParseSchemeTable();
            if (i == TotalPages) break;
            operationZone = Driver.FindElement(By.XPath(PageXPath.SchemePageOperationZone));
            operationZone.FindElement(By.LinkText("Next")).Click();
            MaxPageTimeoutWait.Until((x) => ((IJavaScriptExecutor)Driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        Console.Write("Finish");
    }

    public void ParseSchemeTable()
    {
        TablePage page = new TablePage(PageXPath.SchemeTablePath);
        tableManip.ReadTable(page.table);
    }

    public static void ParallelReadCourseTable(object? page_arg)
    {
        int page = (int)(page_arg ?? throw new ArgumentNullException(nameof(page_arg)));
        if (page < 0) return;
        ChromeDriver TableDriver = new();
    }

    public void IAAALogin(string username, string password)
    {
        try
        {
            Driver.Navigate().GoToUrl(ElectiveUrl.Host);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new SharkError((int)SharkExitCodes.CONNECTIVITY_ERROR);
        }

        MaxPageTimeoutWait.Until((x) => ((IJavaScriptExecutor)Driver).ExecuteScript("return document.readyState").Equals("complete"));
        //identify element then obtain text
        Driver.FindElement(By.Id("user_name")).SendKeys(username);
        Driver.FindElement(By.Id("password")).SendKeys(password);
        Driver.FindElement(By.Id("logon_button")).Click();
        if (Driver.Url.StartsWith(ElectiveUrl.IAAALogin))
        {
            try
            {
                var msg = Driver.FindElement(By.Id("msg"));
                if (msg.Text.Contains(NotificationText.WrongCredential))
                    throw new SharkError((int)SharkExitCodes.CREDENTIAL_ERROR);
            }
            catch (NoSuchElementException)
            {
            }
        }

        try
        {
            MaxPartialPageTimeoutWait.Until(x => Driver.Url.Contains(ElectiveUrl.Host));
            OnLogin?.Invoke();
        }
        catch (TimeoutException e)
        {
            throw new SharkError((int)SharkExitCodes.CREDENTIAL_ERROR);
        }
    }
}