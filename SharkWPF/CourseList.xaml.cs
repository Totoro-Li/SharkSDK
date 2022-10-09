using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using SharkSDK;

namespace SharkWPF
{
    public partial class CourseList
    {
        public CourseListListener Listener { get; set; }

        public CourseListViewModel ViewModel { get; set; }

        public CourseList()
        {
            InitializeComponent();
            Listener = new(this);
            Shared.helper.SetListener(Listener);
            ViewModel = new();
            DataContext = ViewModel;
        }

        private void ListView_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }

    public class CourseListListener : ISharkListener
    {
        public CourseList BindPage { get; set; }

        public CourseListListener(CourseList bindPage)
        {
            BindPage = bindPage;
            OnCourseListReady += OnCourseListReadyImpl;
        }
        public void OnCourseListReadyImpl(List<Course> courses)
        {
            BindPage.ViewModel.CourseList = new();
            BindPage.ViewModel.OnCourseListReady(courses);
            BindPage.CourseDataGrid.Items.Refresh();
        }
    }

    public class CourseListViewModel : ViewModelBase
    {
        private ObservableCollection<Course> _courseList;
        
        public ObservableCollection<Course> CourseList
        {
            get => _courseList;
            set
            {
                _courseList = value;
                OnPropertyChanged();
            }
        }

        public void OnCourseListReady(List<Course> courses)
        {
            CourseList = new ObservableCollection<Course>(courses);
        }
    }
}