using Practice2.Model;
using Practice2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Practice2.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditEmployee.xaml
    /// </summary>
    public partial class EditEmployee : Page
    {
        private Employee _employee;
        public EditEmployee(long employeeId)
        {
            InitializeComponent();
            LoadEmployeeData(employeeId);
        }

        private void LoadEmployeeData(long employeeId)
        {
            using (var db = Helper.GetContext())
            {
                _employee = db.Employee.Find(employeeId);

                if (_employee != null)
                {
                    tbFirstName.Text = _employee.First_name.ToString();
                    tbLastName.Text = _employee.Second_name.ToString();
                    tbMiddleName.Text = _employee.Patronymic.ToString();
                    tbPhoneNumber.Text = _employee.Mobile_number.ToString();
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = Helper.GetContext())
                {
                    var existingEmployee = db.Employee.Find(_employee.Employee_code);

                    if (existingEmployee != null)
                    {
                        existingEmployee.First_name = tbFirstName.Text;
                        existingEmployee.Second_name = tbLastName.Text;
                        existingEmployee.Patronymic = tbMiddleName.Text;
                        existingEmployee.Mobile_number = tbPhoneNumber.Text;
                        db.SaveChanges();
                        MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Сотрудник не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этого сотрудника?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = Helper.GetContext())
                    {
                        var employeeToDelete = db.Employee.Find(_employee.Employee_code);

                        if (employeeToDelete != null)
                        {
                            db.Employee.Remove(employeeToDelete);
                            db.SaveChanges();

                            MessageBox.Show("Сотрудник успешно удалён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            NavigationService.GoBack();
                        }
                        else
                        {
                            MessageBox.Show("Сотрудник не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
