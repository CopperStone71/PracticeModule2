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
    /// Логика взаимодействия для Autho.xaml
    /// </summary>
    public partial class Autho : Page
    {
        int click;
        public Autho()
        {
            InitializeComponent();
            click = 0;
        }

        private void btnEnterGuests_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client(null));
        }

        private void GenerateCapctcha()
        {
            txtboxCaptcha.Visibility = Visibility.Visible;
            txtBlockCaptcha.Visibility = Visibility.Visible;

            string capctchaText = CaptchaGenerator.GenerateCaptchaText(6);
            txtBlockCaptcha.Text = capctchaText;
            txtBlockCaptcha.TextDecorations = TextDecorations.Strikethrough;
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            click += 2;
            string login = txtLogin.Text.Trim();
            string password = tbPassword.Text.Trim();
            string hashPassw = Hash.HashHelper.HashPassword(password);

            prog_comEntities db = Helper.GetContext();

            var user = db.Employee_registration.Where(x => x.Login == login && x.Password == hashPassw).FirstOrDefault();
            if (click == 1)
            {
                if (user != null)
                {
                    MessageBox.Show("Вы вошли под: " + user.Login.ToString());
                    LoadPage(user);
                }
                else
                {
                    MessageBox.Show("Вы ввели логин или пароль неверно!");
                    GenerateCapctcha();

                    tbPassword.Clear();

                    txtBlockCaptcha.Visibility = Visibility.Visible;
                    txtBlockCaptcha.Text = CaptchaGenerator.GenerateCaptchaText(6);
                }
            }
            else if (click > 1)
            {
                if (user != null && txtboxCaptcha.Text == txtBlockCaptcha.Text)
                {
                    txtLogin.Clear();
                    tbPassword.Clear();
                    txtBlockCaptcha.Text = "Text";
                    txtboxCaptcha.Text = "";
                    txtboxCaptcha.Visibility = Visibility.Hidden;
                    txtBlockCaptcha.Visibility = Visibility.Hidden;
                    MessageBox.Show("Вы вошли под: " + user.Login.ToString());
                    LoadPage(user);
                }
                else
                {

                    txtBlockCaptcha.Text = CaptchaGenerator.GenerateCaptchaText(6);
                    txtboxCaptcha.Text = "";
                    MessageBox.Show("Пройдите капчу заново!");
                }
            }
        }

        private void LoadPage( Employee_registration user)
        {
            click = 0;
            NavigationService.Navigate(new Client(user));
        }
    }
}
