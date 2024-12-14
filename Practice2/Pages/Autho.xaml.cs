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
using System.Windows.Threading;

namespace Practice2.Pages
{
    /// <summary>
    /// Логика взаимодействия для Autho.xaml
    /// </summary>
    public partial class Autho : Page
    {
        int click;
        int lockTime;
        DispatcherTimer lockTimer;
        public Autho()
        {
            InitializeComponent();
            click = 0;
            lockTimer = new DispatcherTimer();
            lockTimer.Interval = TimeSpan.FromSeconds(1);
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
            // login = Trax17
            // password = Yuzu
            click += 1;
            string login = txtLogin.Text.Trim();
            string password = tbPassword.Text.Trim();
            string hashPassw = Hash.HashHelper.HashPassword(password);

            prog_comEntities db = Helper.GetContext();

            var user = db.Employee_registration.Where(x => x.Login == login && x.Password == hashPassw).FirstOrDefault();
            if (click == 1)
            {
                if (user != null)
                {        
                    if (IsWithinWorkingHours() == true)
                    {
                        MessageBox.Show("Вы вошли под: " + user.Login.ToString());
                        LoadPage(user);
                    }
                    else
                    {
                        MessageBox.Show("Попытка входа в не рабочее время");
                        Application.Current.Shutdown();
                    }
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
            else if (click > 1 && click < 3)
            {
                if (user != null && txtboxCaptcha.Text == txtBlockCaptcha.Text)
                {
                    txtLogin.Clear();
                    tbPassword.Clear();
                    txtBlockCaptcha.Text = "Text";
                    txtboxCaptcha.Text = "";
                    txtboxCaptcha.Visibility = Visibility.Hidden;
                    txtBlockCaptcha.Visibility = Visibility.Hidden;
                    if (user != null)
                    {
                        if (IsWithinWorkingHours() == true)
                        {
                            MessageBox.Show("Вы вошли под: " + user.Login.ToString());
                            LoadPage(user);
                        }
                        else
                        {
                            MessageBox.Show("Попытка входа в не рабочее время");
                            Application.Current.Shutdown();
                        }
                    }
                }
                else
                {

                    txtBlockCaptcha.Text = CaptchaGenerator.GenerateCaptchaText(6);
                    txtboxCaptcha.Text = "";
                    MessageBox.Show("Пройдите капчу заново!");
                }
            }
            else if (click >= 3)
            {
                if (user != null && txtboxCaptcha.Text == txtBlockCaptcha.Text)
                {
                    txtLogin.Clear();
                    tbPassword.Clear();
                    txtBlockCaptcha.Text = "Text";
                    txtboxCaptcha.Text = "";
                    txtboxCaptcha.Visibility = Visibility.Hidden;
                    txtBlockCaptcha.Visibility = Visibility.Hidden;
                    if (user != null)
                    {
                        if (IsWithinWorkingHours() == true)
                        {
                            MessageBox.Show("Вы вошли под: " + user.Login.ToString());
                            LoadPage(user);
                        }
                        else
                        {
                            MessageBox.Show("Попытка входа в не рабочее время");
                            Application.Current.Shutdown();
                        }
                    }
                }
                else
                {
                    btnEnter.IsEnabled = false;
                    btnEnterGuests.IsEnabled = false;
                    txtLogin.IsEnabled = false;
                    tbPassword.IsEnabled = false;
                    txtboxCaptcha.IsEnabled = false;
                    txtboxCaptcha.Text = "";

                    lockTime = 10;
                    txtBlockTimer.Text = $"Повторите через {lockTime} секунд";
                    txtBlockTimer.Visibility = Visibility.Visible;

                    lockTimer.Tick += LockTimer_Tick;
                    lockTimer.Start();
                }
            }
        }

        private void LockTimer_Tick(object sender, EventArgs e)
        {
            lockTime--;
            txtBlockTimer.Text = $"Повторите через {lockTime} секунд";

            if (lockTime <= 0)
            {
                lockTimer.Stop();
                btnEnter.IsEnabled = true;
                btnEnterGuests.IsEnabled = true;
                txtLogin.IsEnabled = true;
                tbPassword.IsEnabled = true;
                txtboxCaptcha.IsEnabled = true;

                txtBlockTimer.Visibility = Visibility.Hidden;
                click = 0;
                lockTimer.Tick -= LockTimer_Tick;
            }
        }
            private void LoadPage(Employee_registration user)
        {
            click = 0;
            var clientPage = new Client(user);
            NavigationService.Navigate(clientPage);
        }

        private bool IsWithinWorkingHours()
        {
            int currentHour = DateTime.Now.Hour;
            return currentHour >= 10 && currentHour < 19;
        }
    }
}
