using System.Reflection;
using System.Runtime.CompilerServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Events;

namespace SharkSDK
{
    public class Base
    {
        private static IWebDriver? _driverInternal;
        protected static EventFiringWebDriver? Driver { get; set; }

        protected static  void SetDriver(IWebDriver? driver)
        {
            _driverInternal = driver;
            Driver = new(_driverInternal);
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
        public List<StructTypes.Course> CourseDataCollection { get; set; } = new();

        public void ReadTable(IWebElement table)
        {
            //Get all the columns from the table
            var columns = table.FindElements(By.TagName("th"));

            //Get all the rows
            var rows = table.FindElements(By.TagName("tr"));

            foreach (var row in rows)
            {
                var Columns = row.FindElements(By.TagName("td"));
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

        static StructTypes.Course GetCourse(IReadOnlyCollection<IWebElement> columns)
        {
            PropertyInfo[] Properties = typeof(StructTypes.Course).GetProperties();
            int i = 0;
            StructTypes.Course res = new();
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
                    if (Properties[i].PropertyType.ToString() == "System.ValueTuple`2[System.Int32,System.Int32]")
                    {
                        List<int> conv = block.Text.Split(Separators.QuotaSeparator).Select(x => int.Parse(x)).ToList();
                        if (conv.Count() != 2)
                            throw new SharkError((int)SharkExitCodes.PARSING_ERROR);
                        (int, int) quota = (conv.ElementAt(0), conv.ElementAt(1));
                        Properties[i].SetValue(boxedRes, Convert.ChangeType(quota, Properties[i].PropertyType), null);
                    }
                    else
                        Properties[i].SetValue(boxedRes, Convert.ChangeType(block.Text, Properties[i].PropertyType), null);
                }
                else
                    throw new SharkError((int)SharkExitCodes.PARSING_ERROR);

                ++i;
            }

            return (StructTypes.Course)boxedRes;
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