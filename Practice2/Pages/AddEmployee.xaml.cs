using Practice2.Model;
using Practice2.Services;
using System;
using System.Collections.Generic;
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
