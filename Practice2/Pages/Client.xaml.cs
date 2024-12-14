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
using Practice2.Model;
using Practice2.Services;

namespace Practice2.Pages
{
    public partial class Client : Page
    {
        private Employee_registration _getUser;

        public Client(Employee_registration user)
        {
            InitializeComponent();
            _getUser = user;
            LoadPage();
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

        /*private void Grid_Initialized(object sender, EventArgs e)
        {
            prog_comEntities db = Helper.GetContext();
            var user = db.Employee_registration.Where(x => x.Login == _getUser.Login);
            if (_getUser != null)
            {
                string greetingMessage = GetGreeting(_getUser.Login);
                txtblc_Welcome.Text = greetingMessage;
            }
            else
            {
                txtblc_Welcome.Text = "Пользователь не найден.";
            }
        }*/

        private void LoadPage() 
        {
            prog_comEntities db = Helper.GetContext();
            var user = db.Employee_registration.Where(x => x.Login == _getUser.Login);
            if (_getUser != null)
            {
                string greetingMessage = GetGreeting(_getUser.Login);
                txtblc_Welcome.Text = greetingMessage;
            }
            else
            {
                txtblc_Welcome.Text = "Пользователь не найден.";
            }
        }
        
    }
}
