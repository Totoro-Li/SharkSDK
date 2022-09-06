using System.Collections.ObjectModel;
using System.Security;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;

namespace SharkSDK;

public partial class BrowserHelper : Base
{
    private ICollection<Cookie>? SessionCookie { get; set; }

    private void OnLogin()
    {
        OnSuccessfulLoginImpl();
        _listeners.ForEach(x=>x.OnLogin?.Invoke());
    }

    public void OnCourseListReady()
    {
        _listeners.ForEach(x=>x.OnCourseListReady?.Invoke(CourseList));
    }

    

    private TableManip tableManip { get; set; }

    private WebDriverWait MaxPageTimeoutWait { get; set; }

    WebDriverWait MaxPartialPageTimeoutWait { get; set; }

    
    private List<ISharkListener> _listeners = new();

    public int TotalPages { get; set; } = 1;
    public List<Course> CourseList => tableManip.CourseDataCollection;

    public List<Course> DesiredCourseList { get; set; } = new();

    private ObservableCollection<Thread> _threadPool = new();

    private (string, SecureString) Credential { get; set; }

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
            OnTableReady();
    }

    public void SetCredential(string username, SecureString password)
    {
        Credential = (username, password)!;
    }


    public void OnTableReady()
    {
        if (Driver == null) throw new SharkError((int)SharkExitCodes.DRIVER_ERROR);
        IWebElement operationZone = Driver.FindElement(By.XPath(PageXPath.SchemePageOperationZone));
        var pageText = operationZone.Text;
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
        OnCourseListReady();
        Console.Write("Finish Course List Scraping");
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

    public void AddDesiredCourse(Course desired) => DesiredCourseList.Add(desired);

    public void SetListener(ISharkListener listener)
    {
        if (_listeners.Contains(listener))
            return;
        _listeners.Add(listener);
    }

    public void IAAALogin()
    {
        if (Driver == null) throw new SharkError((int)SharkExitCodes.DRIVER_ERROR);
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
        Driver.FindElement(By.Id("user_name")).SendKeys(Credential.Item1);
        Driver.FindElement(By.Id("password")).SendKeys(Utilities.SecureStringToString(Credential.Item2));
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
            OnLogin();
        }
        catch (TimeoutException e)
        {
            throw new SharkError((int)SharkExitCodes.CREDENTIAL_ERROR);
        }
    }
}