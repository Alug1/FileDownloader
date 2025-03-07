using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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

namespace ClassLibrary1.Element
{
    /// <summary>
    /// Логика взаимодействия для UserControl2.xaml
    /// </summary>
    public partial class UserControl2 : Window
    {
        public UserControl2()
        {
            InitializeComponent();
            string log = File.ReadAllLines(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\Seting.ini").ElementAt(0);
            log = log.Replace("Логин:", "");
            string pas = File.ReadAllLines(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\Seting.ini").ElementAt(1);
            pas = pas.Replace("Пароль:", "");
            tbLogin.Text = log;
            tbPassword.Text = pas;
            if (File.ReadAllLines(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\Seting.ini").ElementAt(4) == "Запомнить:да") cbSave.IsChecked = true;

        }
        static async void FunTask(string username,string password)
        {
            //Console.Write("Введите логин: ");
            /*string username = File.ReadAllLines(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\Seting.ini").ElementAt(0);
            username = username.Replace("Логин:", "");
            //Console.Write("Введите пароль: ");
            string password = File.ReadAllLines(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\Seting.ini").ElementAt(1);
            password = password.Replace("Пароль:", "");*/
            string loginUrl = "https://cloud.tavrida.ru/index.php/login";
            string homeUrl = "https://cloud.tavrida.ru/"; // Главная страница после входа

            // Создаем HttpClient с поддержкой куки
            using (var handler = new HttpClientHandler { AllowAutoRedirect = true, UseCookies = true, CookieContainer = new System.Net.CookieContainer() })
            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                try
                {
                    // Шаг 1: Получаем страницу логина и requesttoken
                    HttpResponseMessage initialResponse = await client.GetAsync(loginUrl);
                    string initialHtml = await initialResponse.Content.ReadAsStringAsync();
                    string requestToken = ExtractRequestToken(initialHtml);

                    /*if (string.IsNullOrEmpty(requestToken))
                    {
                        Console.WriteLine("Не удалось найти requesttoken.");
                        Console.WriteLine($"Содержимое ответа: {initialHtml.Substring(0, Math.Min(500, initialHtml.Length))}...");
                        return;
                    }
                    Console.WriteLine($"RequestToken: {requestToken}");*/

                    // Шаг 2: Отправляем POST-запрос для авторизации
                    var formData = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("user", username),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("requesttoken", requestToken),
                    new KeyValuePair<string, string>("timezone", "Europe/Moscow"), // Опционально, для совместимости
                    new KeyValuePair<string, string>("timezone_offset", "3")     // Опционально
                });

                    HttpResponseMessage loginResponse = await client.PostAsync(loginUrl, formData);
                    string loginHtml = await loginResponse.Content.ReadAsStringAsync();

                    if (loginResponse.IsSuccessStatusCode)
                    {
                        // Шаг 3: Запрашиваем главную страницу после входа
                        HttpResponseMessage homeResponse = await client.GetAsync(homeUrl);
                        string homeHtml = await homeResponse.Content.ReadAsStringAsync();

                        string guid = ExtractGuid(homeHtml);
                        if (!string.IsNullOrEmpty(guid))
                        {
                            // Console.WriteLine($"GUID пользователя {username}: {guid}");
                            string[] text = File.ReadAllLines(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\Seting.ini");
                            text[2] = "GUID:" + guid;
                            File.WriteAllLines(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\Seting.ini", text);
                        }
                        else
                        {
                           // Console.WriteLine("GUID не найден в HTML.");
                            //Console.WriteLine($"Содержимое главной страницы: {homeHtml.Substring(0, Math.Min(500, homeHtml.Length))}...");
                        }
                    }
                    else
                    {
                        //Console.WriteLine($"Ошибка авторизации: {loginResponse.StatusCode} - {loginResponse.ReasonPhrase}");
                        MessageBox.Show("Ошибка авторизации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        //Console.WriteLine($"Содержимое ответа: {loginHtml.Substring(0, Math.Min(500, loginHtml.Length))}...");
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"Ошибка: {ex.Message}");
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        static string ExtractRequestToken(string html)
        {
            int tokenIndex = html.IndexOf("data-requesttoken=\"");
            if (tokenIndex != -1)
            {
                int start = tokenIndex + "data-requesttoken=\"".Length;
                int end = html.IndexOf("\"", start);
                return html.Substring(start, end - start);
            }
            return null;
        }
        static string ExtractGuid(string html)
        {
            int dataUserIndex = html.IndexOf("data-user=\"");
            if (dataUserIndex != -1)
            {
                int start = dataUserIndex + "data-user=\"".Length;
                int end = html.IndexOf("\"", start);
                return html.Substring(start, end - start);
            }
            return null;
        }




        private void btLoginOk_Click(object sender, RoutedEventArgs e)
        {
            //string[] UserLog = {tbLogin.Text,tbPassword.Text };
           string pas=tbPassword.Text,log=tbLogin.Text;
            pas=pas.Trim(' ');
            log =log.Trim(' ');
            if (pas != "" && log != "")
            {
                string[] text = File.ReadAllLines(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\Seting.ini");
                text[0] = "Логин:" + tbLogin.Text;
                text[1] = "Пароль:" + tbPassword.Text;
                if (cbSave.IsChecked == true) text[4] = "Запомнить:" + "да";
                else text[4]= "Запомнить:" + "нет";
                File.WriteAllLines(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\Seting.ini", text);
                FunTask(tbLogin.Text,tbPassword.Text);
                Close();
            }
            else
            {
                MessageBox.Show($"Введите данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tbLogin_Initialized(object sender, EventArgs e)
        {
            
            //tbLogin.Text=File.ReadAllLines(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\Seting.ini").ElementAt(0);
        }
    }
}
