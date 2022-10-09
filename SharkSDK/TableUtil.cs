using System.Reflection;
using System.Runtime.CompilerServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Events;

namespace SharkSDK
{
    public class Base
    {
        private static IWebDriver? _driverInternal;
        public static EventFiringWebDriver? Driver { get; set; }
        
        public event EventHandler? CourseListChanged;

        public void OnCourseChanged()
        {
            if(Driver!=null) CourseListChanged?.Invoke(this,EventArgs.Empty);
        }

        protected static void SetDriver(IWebDriver? driver)
        {
            if (driver != null)
            {
                _driverInternal = driver;
                Driver = new(_driverInternal);
                Driver.ExceptionThrown += Listeners.DriverException_Handler;
                Driver.Navigated += Listeners.DriverNavigated_Handler;
            }
        }
    }

    public class TablePage : Base
    {
        public TablePage(string tablePath)
        {
            table = Driver.FindElement(By.XPath(tablePath));
        }

        public IWebElement table { get; set; }
    }


    public class TableManip
    {
        public List<Course> CourseDataCollection { get; set; } = new();

        public void ReadTable(IWebElement table)
        {
            //Get all the columns from the table
            var columns = table.FindElements(By.TagName("th"));

            //Get all the rows
            var rows = table.FindElements(By.TagName("tr"));

            foreach (var row in rows)
            {
                var ElementColumns = row.FindElements(By.TagName("td"));
                List<string> Columns = ElementColumns.Select(x=>x.Text).SelectMany( 
                    i => i.Split( Separators.QuotaSeparator ).Select( v => v.Trim() ) 
                ).ToList();
                if (Columns.Count > 5)
                    CourseDataCollection.Add(GetCourse(Columns));
            }
        }

        #region Utils

        static void PropertySet(object p, string propName, object value)
        {
            Type t = p.GetType();
            PropertyInfo info = t.GetProperty(propName);
            if (info == null)
                return;
            if (!info.CanWrite)
                return;
            info.SetValue(p, value, null);
        }

        static void PropertySetLooping(object p, string propName, object value)
        {
            Type t = p.GetType();
            foreach (PropertyInfo info in t.GetProperties())
            {
                if (info.Name == propName && info.CanWrite)
                {
                    info.SetValue(p, value, null);
                }
            }
        }

        static Course GetCourse(List<string> columns)
        {
            PropertyInfo[] Properties = typeof(Course).GetProperties();
            int i = 0;
            Course res = new();
            object boxedRes = RuntimeHelpers.GetObjectValue(res);
            foreach (var block in columns)
            {
                if (i >= Properties.Length) break;
                /*
                object? val = Properties[i].PropertyType.ToString() switch 
                {
                    "System.String"=>block.Text.ToString(),
                    "Int32" => int.Parse(block.Text),
                    "Double" =>double.Parse(block.Text),
                    _ => null
                };
                */
                if (Properties[i].CanWrite)
                {
                    Properties[i].SetValue(boxedRes, Convert.ChangeType(block, Properties[i].PropertyType), null);
                }
                else
                    throw new SharkError((int)SharkExitCodes.PARSING_ERROR);
                ++i;
            }

            return (Course)boxedRes;
        }

        #endregion

        //..
    }

    public class TableDataCollection
    {
        public int RowNumber { get; set; }

        public string ColumnName { get; set; }

        public string ColumnValue { get; set; }

        //For Complex tables
        public IEnumerable<IWebElement> ColumnSpecialValues { get; set; }
        //..
    }
}