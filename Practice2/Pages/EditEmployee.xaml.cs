using Practice2.Model;
using Practice2.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using System.Xml.Linq;

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
            StringBuilder errorMessages = new StringBuilder();

            /*if (string.IsNullOrWhiteSpace(tbFirstName.Text))
            {
                errorMessages.AppendLine("Поле \"Имя\" обязательно для заполнения.");
            }
            else if (tbFirstName.Text.Length < 3)
            {
                errorMessages.AppendLine("Имя должно содержать не менее 3 букв.");
            }

            if (string.IsNullOrWhiteSpace(tbLastName.Text))
            {
                errorMessages.AppendLine("Поле \"Фамилия\" обязательно для заполнения.");
            }
            else if (tbLastName.Text.Length < 3)
            {
                errorMessages.AppendLine("Фамилия должна содержать не менее 3 букв.");
            }

            if (!string.IsNullOrWhiteSpace(tbMiddleName.Text) && tbMiddleName.Text.Length < 3)
            {
                errorMessages.AppendLine("Отчество должно содержать не менее 3 букв, если указано.");
            }

            if (errorMessages.Length > 0)
            {
                MessageBox.Show(errorMessages.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(tbFirstName.Text) || string.IsNullOrWhiteSpace(tbLastName.Text))
            {
                MessageBox.Show("Заполните обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }*/

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

                        var validationContext = new ValidationContext(existingEmployee, null, null);
                        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                        StringBuilder sb = new StringBuilder();
                        if (!Validator.TryValidateObject(existingEmployee, validationContext, validationResults, true))
                        {
                            foreach (var error in validationResults)
                            {
                                sb.AppendLine(error.ErrorMessage);
                            }
                            MessageBox.Show(sb.ToString());
                            return;
                        }

                        /*bool isValid = Validator.TryValidateObject(existingEmployee, validationContext, validationResults, true);
                        if (!isValid)
                        {
                            string errorMessages = string.Join("\n", validationResults.Select(r => r.ErrorMessage));
                            MessageBox.Show($"Ошибки валидации:\n{errorMessages}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }*/

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
