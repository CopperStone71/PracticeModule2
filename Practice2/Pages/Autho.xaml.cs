using Practice2.Model;
using Practice2.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Net;

namespace Practice2.Pages
{
    ///<summary>
    /// Стартовое окно с авторизацией пользователя
    ///</summary>
    public partial class Autho : Page
    {
        int click;
        int lockTime;
        DispatcherTimer lockTimer;
        private string generatedCode;
        private string userEmail;
        private string userLogin;

        public Autho()
        {
            InitializeComponent();
            click = 0;
            lockTimer = new DispatcherTimer();
            lockTimer.Interval = TimeSpan.FromSeconds(1);
        }

        private void GenerateCaptcha() // метод генерирующий Капчу 
        {
            txtboxCaptcha.Visibility = Visibility.Visible;
            txtBlockCaptcha.Visibility = Visibility.Visible;

            string captchaText = CaptchaGenerator.GenerateCaptchaText(6);
            txtBlockCaptcha.Text = captchaText;
            txtBlockCaptcha.TextDecorations = TextDecorations.Strikethrough;
        }

        private void LockTimer_Tick(object sender, EventArgs e) // метод блокировки ввода на определенное время
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
            return currentHour >= 10 && currentHour < 20;
        }

        private void btnEnterGuests_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client(null));
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            click += 1;
            string login = txtLogin.Text.Trim();
            string password = tbPassword.Text.Trim();
            string hashPassw = Hash.HashHelper.HashPassword(password);

            prog_comEntities db = Helper.GetContext();

            var user = db.Employee_registration.Where(x => x.Login == login && x.Password == hashPassw).FirstOrDefault(); //проверка соответствия логина и пароля существующим в базе данных
            /*
            При одном неправильном вводе генерирует капчу,
            При более трех неправильных вводах блокирует ввод на 30 секунд
            */
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
                        MessageBox.Show("Вы вошли под: " + user.Login.ToString());
                        LoadPage(user);
                    }
                }
                else
                {
                    MessageBox.Show("Вы ввели логин или пароль неверно!");
                    GenerateCaptcha();

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

        private void txtLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                btnLostPassword.Visibility = Visibility.Visible;
            }
            else
            {
                btnLostPassword.Visibility = Visibility.Hidden;
            }
        }
        private void btnLostPassword_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                tblEmail.Visibility = Visibility.Visible;
                tbEmail.Visibility = Visibility.Visible;
                btnSendCode.Visibility = Visibility.Visible;
            }
        }
        private void btnSendCode_Click(object sender, RoutedEventArgs e) // метод отправки сгенерированного пароля на написанный адрес электронной почты
        {
            userEmail = tbEmail.Text.Trim();
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                MessageBox.Show("Введите адрес электронной почты.");
                return;
            }

            generatedCode = CaptchaGenerator.GenerateCaptchaText(6);

            try
            {
                SendEmail(userEmail, "Код восстановления пароля", $"Ваш код: {generatedCode}");
                MessageBox.Show("Код отправлен на вашу почту.");

                txtBlockCaptcha.Text = "Код:";
                txtBlockCaptcha.Visibility = Visibility.Visible;
                txtboxCaptcha.Visibility = Visibility.Visible;
                btnCheckCode.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке кода: {ex.Message}");
            }
        }
        private void btnCheckCode_Click(object sender, RoutedEventArgs e)
        {
            string enteredCode = txtboxCaptcha.Text.Trim();
            if (enteredCode == generatedCode)
            {
                tblNewPassword.Visibility = Visibility.Visible;
                tbNewPassword.Visibility = Visibility.Visible;
                btnChangePassword.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Неверный код. Попробуйте снова.");
            }
        }
        private void btnChangePassword_Click(object sender, RoutedEventArgs e) // метод смены пароля пользователя
        {
            string newPassword = tbNewPassword.Text.Trim();
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Введите новый пароль.");
                return;
            }

            using (var db = Helper.GetContext())
            {
                var user = db.Employee_registration.FirstOrDefault(x => x.Login == txtLogin.Text);
                if (user != null)
                {
                    user.Password = Hash.HashHelper.HashPassword(newPassword);
                    db.SaveChanges();
                    MessageBox.Show("Пароль успешно изменен.");
                }
                else
                {
                    MessageBox.Show("Пользователь не найден.");
                }
            }
        }
        ///<summary>
        /// метод отправки сообщения на электронную почту
        ///</summary>
        ///<param name="to">Объект хранящий значение адреса электронной почты пользователя</param>
        ///<param name="subject">Объект хранящий текст темы электронного письма</param>
        ///<param name="body">Объект хранящий текст письма</param>
        private void SendEmail(string to, string subject, string body)
        {
            using (SmtpClient smtpClient = new SmtpClient("smtp.mail.ru", 587))
            {
                smtpClient.Credentials = new NetworkCredential("csharp_test@internet.ru", "JTz7wkbyYkCy5YWU6K8z");
                smtpClient.EnableSsl = true;

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("csharp_test@internet.ru"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };
                mailMessage.To.Add(to);

                smtpClient.Send(mailMessage);
            }
        }
    }
}