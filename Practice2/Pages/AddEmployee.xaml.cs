using Practice2.Model;
using Practice2.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
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
    /// Логика взаимодействия для AddEmployee.xaml
    /// </summary>
    public partial class AddEmployee : Page
    {
        public AddEmployee()
        {
            InitializeComponent();
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errorMessages = new StringBuilder();
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                errorMessages.AppendLine("Поле \"Имя\" обязательно для заполнения.");
            }
            else if (tbName.Text.Length < 3)
            {
                errorMessages.AppendLine("Имя должно содержать не менее 3 букв.");
            }

            if (string.IsNullOrWhiteSpace(tbSurname.Text))
            {
                errorMessages.AppendLine("Поле \"Фамилия\" обязательно для заполнения.");
            }
            else if (tbSurname.Text.Length < 3)
            {
                errorMessages.AppendLine("Фамилия должна содержать не менее 3 букв.");
            }

            if (!string.IsNullOrWhiteSpace(tbPatronumic.Text) && tbPatronumic.Text.Length < 3)
            {
                errorMessages.AppendLine("Отчество должно содержать не менее 3 букв, если указано.");
            }

            if (errorMessages.Length > 0)
            {
                MessageBox.Show(errorMessages.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(tbName.Text) || string.IsNullOrWhiteSpace(tbSurname.Text))
            {
                MessageBox.Show("Заполните обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newEmployee = new Employee
                {
                    First_name = tbName.Text,
                    Second_name = tbSurname.Text,
                    Patronymic = tbPatronumic.Text,
                    Mobile_number = tbPhoneNumber.Text
                };

                var validationContext = new ValidationContext(newEmployee);
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                StringBuilder sb = new StringBuilder();
                if (!Validator.TryValidateObject(newEmployee, validationContext, validationResults, true))
                {
                    foreach (var error in validationResults)
                    {
                        sb.AppendLine(error.ErrorMessage);
                    }
                    MessageBox.Show(sb.ToString());
                    return;
                }

                /*bool isValid = Validator.TryValidateObject(newEmployee, validationContext, validationResults, true);
                if (!isValid)
                {
                    string errorMessages = string.Join("\n", validationResults.Select(r => r.ErrorMessage));
                    MessageBox.Show($"Ошибки валидации:\n{errorMessages}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }*/

                using (var context = Helper.GetContext())
                {
                    context.Database.Log = Console.WriteLine;
                    context.Employee.Add(newEmployee);
                    context.SaveChanges();
                }

                MessageBox.Show("Сотрудник успешно добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (DbUpdateException dbEx)
            {
                MessageBox.Show($"Ошибка при добавлении сотрудника: {dbEx.InnerException?.Message ?? dbEx.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
