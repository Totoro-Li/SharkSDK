using System.Reflection;


namespace SharkSDK;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class BrowserHelper
{
    public ICollection<Cookie> SessionCookie { get; set; }

    public delegate void OnLoadResp();

    public OnLoadResp? OnLogin;

    public OnLoadResp? OnTableReady;

    public delegate void OnCourseAvailableResp();

    private ChromeDriver driver { get; set; }

    public BrowserHelper()
    {
        //options.AddArguments("-headless");
        driver = new ChromeDriver();
        OnLogin = OnSuccessfulLoginImpl;
        OnTableReady = OnTableReadyImpl;
    }

    public void OnSuccessfulLoginImpl()
    {
        SharkExitCodes status = SharkExitCodes.GENERAL_ERROR;
        SessionCookie = driver.Manage().Cookies.AllCookies;
        WebDriverWait wait = new WebDriverWait(this.driver, TimeSpan.FromSeconds(Timeouts.MaxPageTimeout));
        for (int attempt = 0; attempt < Timeouts.MaxAttempts; ++attempt)
        {
            driver.Navigate().Refresh();
            wait.Until((x) => ((IJavaScriptExecutor)this.driver).ExecuteScript("return document.readyState").Equals("complete"));
            try
            {
                driver.FindElement(By.LinkText(NotificationText.AddOrDropLinkText)).Click();
            }
            catch (NoSuchElementException)
            {
                continue;
            }
            if (driver.Title == NotificationText.SystemError)
            {
                driver.Navigate().Back();
                continue;
            }

            if (driver.Title == NotificationText.AddOrDropLinkText)
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
        
    }

    public void IAAALogin(string username, string password)
    {
        try
        {
            driver.Navigate().GoToUrl(ElectiveUrl.Host);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new SharkError((int)SharkExitCodes.CONNECTIVITY_ERROR);
        }

        WebDriverWait wait = new WebDriverWait(this.driver, TimeSpan.FromSeconds(Timeouts.MaxPageTimeout));
        wait.Until((x) => ((IJavaScriptExecutor)this.driver).ExecuteScript("return document.readyState").Equals("complete"));
        //identify element then obtain text
        driver.FindElement(By.Id("user_name")).SendKeys(username);
        driver.FindElement(By.Id("password")).SendKeys(password);
        driver.FindElement(By.Id("logon_button")).Click();
        if (driver.Url.StartsWith(ElectiveUrl.IAAALogin))
        {
            try
            {
                var msg = driver.FindElement(By.Id("msg"));
                if (msg.Text.Contains(NotificationText.WrongCredential))
                    throw new SharkError((int)SharkExitCodes.CREDENTIAL_ERROR);
            }
            catch (NoSuchElementException)
            {
            }
        }

        try
        {
            WebDriverWait fastWait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
            fastWait.Until(ExpectedConditions.UrlContains(ElectiveUrl.Host));
            OnLogin?.Invoke();
        }
        catch (TimeoutException e)
        {
            throw new SharkError((int)SharkExitCodes.CREDENTIAL_ERROR);
        }
    }
}