using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Practice2.Model;
using Practice2.Services;

namespace Practice2.Pages
{
    public partial class Client : Page
    {
        private Employee_registration _getUser;
        private List<Employees> _employees;
        private List<WorkTeams> _workTeams;
        private List<Employees> _filteredEmployees;
        private List<string> _jobTitles;

        public Client(Employee_registration user)
        {
            InitializeComponent();
            _getUser = user;
            LoadPage();
            LoadEmployees();
            LoadWorkTeams();
            LoadJobTitles();
        }

        private string GetGreeting(string user)
        { 
                string timeOfDayGreeting = GetTimeOfDayGreeting();
                return $"Добро пожаловать, {user}!\n{timeOfDayGreeting}";        
        }

        private string GetTimeOfDayGreeting()
        {
            int hour = DateTime.Now.Hour;

            if (hour >= 10 && hour < 12)
                return "Доброе утро!";
            else if (hour >= 12 && hour < 17)
                return "Добрый день!";
            else if (hour >= 17 && hour < 19)
                return "Добрый вечер!";
            else
                return "Приветствую!";
        }

        private void LoadPage() // метод пишущий логин вошедшего пользователя
        {
            prog_comEntities db = Helper.GetContext();
            var user = db.Employee_registration.Where(x => x.Login == _getUser.Login);
            if (_getUser != null)
            {
                string greetingMessage = GetGreeting(_getUser.Login);
                txtFullName.Text = greetingMessage;
            }
            else
            {
                txtFullName.Text = "Пользователь не найден.";
            }
        }
        ///<summary>
        /// метод загружающий в ListView данные о сотрудниках
        ///</summary>
        private void LoadEmployees()
        {
            _employees = Helper.GetContext().Employee.Select(e => new Employees
            {

                Employee_code = e.Employee_code.ToString(),
                First_name = e.First_name,
                Second_name = e.Second_name,
                Patronymic = e.Patronymic,
                Employee_type_code = e.Employee_type.Type,
                Mobile_number = e.Mobile_number.ToString(),
            }).ToList();
            foreach (var employee in _employees)
            {
                employee.FullName = $"{employee.Second_name} {employee.First_name}";
                employee.PhotoUrl = "P:\\Учёба\\Програмные модули(C#)\\Practice2\\Practice2\\Resources\\default_photo.jpg";
            }
            EmployeesListView.ItemsSource = _employees;
        }
        ///<summary>
        /// метод загружающий должности из базы данных
        ///</summary>
        private void LoadJobTitles()
        {
            _jobTitles = Helper.GetContext().Employee_type.Select(j => j.Type).Distinct().ToList();

            _jobTitles.Insert(0, "Все должности");

            cbJobTitle.ItemsSource = _jobTitles;
        }
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterEmployees();
        }

        private void cbJobTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterEmployees();
        }
        ///<summary>
        /// метод фильтрующий по должности
        ///</summary>
        private void FilterEmployees()
        {
            string searchText = tbSearch.Text.ToLower();
            string selectedJobTitle = cbJobTitle.SelectedItem as string;

            _filteredEmployees = _employees.Where(emp =>
                (emp.Second_name + " " + emp.First_name + " " + emp.Patronymic).ToLower().Contains(searchText) &&
                (selectedJobTitle == "Все должности" || emp.Employee_type_code == selectedJobTitle))
                .ToList();

            EmployeesListView.ItemsSource = null;
            EmployeesListView.ItemsSource = _filteredEmployees;
        }
        private void LoadWorkTeams()
        {
                _workTeams = Helper.GetContext().Work_team.Select(e => new WorkTeams
                {
                    TeamName = e.Work_team_name,
                    FirstName = e.Employee.First_name,
                    SecondName = e.Employee.Second_name,
                }).ToList();
                foreach (var employee in _workTeams)
                {
                    employee.TeamName = $"{employee.TeamName}";
                    employee.FullName = $"{employee.SecondName} {employee.FirstName}";
                    employee.TeamPhoto = "P:\\Учёба\\Програмные модули(C#)\\Practice2\\Practice2\\Resources\\team_photo.jpg";
                }
                WorkTeamListView.ItemsSource = _workTeams;
        }
        private void EmployeesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e) // метод открывающий окно с изменением сотрудников на двойной клик по сотруднику
        {
            if (EmployeesListView.SelectedItem is Employees selectedEmployee)
            {
                try
                {
                    long employeeId = long.Parse(selectedEmployee.Employee_code);

                    using (var db = Helper.GetContext())
                    {
                        var employeeExists = db.Employee.Find(employeeId) != null;

                        if (employeeExists)
                        {
                            NavigationService.Navigate(new EditEmployee(employeeId));
                        }
                        else
                        {
                            MessageBox.Show($"Сотрудник с ID = {employeeId} не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("Ошибка формата ID: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEmployee());
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadEmployees();
        }

        private void PDFedButton_Click(object sender, RoutedEventArgs e)
        {
            FlowDocument doc = flowDocumentReader.Document;

            if (doc == null)
            {
                MessageBox.Show("Документ не найден");
                return;
            }
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Список команд");
            }
        }

        private void PDFedEmpButton_Click(object sender, RoutedEventArgs e)
        {
            FlowDocument doc = flowDocumentEmpReader.Document;

            if (doc == null)
            {
                MessageBox.Show("Документ не найден");
                return;
            }
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Список сотрудников");
            }
        }
    }
}
