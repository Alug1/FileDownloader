using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
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
using Autodesk.Revit.UI;
using System.Drawing;
using Autodesk.Revit.DB;
using ClassLibrary1;
using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using System.Diagnostics;
using System.Reflection;


namespace ClassLibrary1.Element
{
    
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>

    public partial class UserControl1 : Window
    {
        private UIApplication _uiApp;
        private string puthDir;
        public UserControl1(UIApplication uiApp)
        {
            InitializeComponent();
            _uiApp = uiApp;
            puthDir= File.ReadAllLines(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\Seting.ini").ElementAt(3);
            puthDir = puthDir.Replace("Путь:", "");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btUserName_Click(object sender, RoutedEventArgs e)
        {
            UserControl2 formUser = new UserControl2();
            formUser.ShowDialog();
        }
       
        private async void searchFamily_ClickAsync(object sender, RoutedEventArgs e)
        {
            // Точный URL для скачивания файла через WebDAV
            string GUID= File.ReadAllLines(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\Seting.ini").ElementAt(2);
            GUID = GUID.Replace("GUID:", "");
            string nameFam = tbSearch.Text+".rfa";
            //string downloadUrl = "https://cloud.tavrida.ru/remote.php/dav/files/2650F160-DA99-48BE-8EE7-356D9565A361/TER_SubUnit_Support_OHL/%D0%A1%D0%B5%D0%BC%D0%B5%D0%B9%D1%81%D1%82%D0%B2%D0%B0%20(%D0%B4%D0%B5%D1%82%D0%B0%D0%BB%D0%B8)/" + nameFam;
            string downloadUrl = "https://cloud.tavrida.ru/remote.php/dav/files/"+GUID+"/TER_SubUnit_Support_OHL/%D0%A1%D0%B5%D0%BC%D0%B5%D0%B9%D1%81%D1%82%D0%B2%D0%B0%20(%D0%B4%D0%B5%D1%82%D0%B0%D0%BB%D0%B8)/" + nameFam;
            //string username = "lugas@tavrida.ru"; // Логин пользователя
            //string password = "Qwerty12345";      // Пароль пользователя
            string username = File.ReadAllLines(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\Seting.ini").ElementAt(0);
            username = username.Replace("Логин:", "");
            string password = File.ReadAllLines(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\Seting.ini").ElementAt(1);
            password = password.Replace("Пароль:", "");
            string localFilePath = System.IO.Path.Combine(puthDir, nameFam);
            string debugFilePath = System.IO.Path.Combine(puthDir, "debug_response.html");
            if(password==""&&username==""&&GUID=="")
            {
                MessageBox.Show("Авторизуйтесь", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Создаем HttpClient
            using (HttpClient client = new HttpClient())
            {
                // Устанавливаем авторизацию (Basic Auth)
                var authToken = Encoding.ASCII.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                try
                {
                    // Выполняем GET-запрос
                    HttpResponseMessage response = await client.GetAsync(downloadUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        if (!File.Exists(localFilePath))
                        {
                            // Сохраняем файл
                            using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                await response.Content.CopyToAsync(fileStream);
                            }
                        }
                        nameFamily.Content = nameFam;
                        creatureDate.Content =  new FileInfo(localFilePath).CreationTime;
                        weightFile.Content = new FileInfo(localFilePath).Length;

                        // Путь для сохранения изображения
                        string outputImagePath = puthDir+"\\Ic32.jpg";

                        // Запуск Revit в фоновом режиме

                        Autodesk.Revit.ApplicationServices.Application revitApp = _uiApp.Application;


                        // Открытие файла RFA в фоновом режиме

                        Document docs = revitApp.OpenDocumentFile(localFilePath);
                        // Создание вида для экспорта
                        View3D view = Find3DView(docs);
                        if (view == null)
                        {
                            MessageBox.Show("3D вид не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // Настройка параметров экспорта изображения
                        ImageExportOptions options = new ImageExportOptions
                        {
                            FilePath = outputImagePath,
                            FitDirection = FitDirectionType.Horizontal,
                            HLRandWFViewsFileType = ImageFileType.JPEGLossless,
                            ImageResolution = ImageResolution.DPI_300,
                            ExportRange = ExportRange.SetOfViews,
                            ZoomType = ZoomFitType.FitToPage,
                            PixelSize = 1024, // Размер изображения
                            ViewName = view.Name // Указываем имя вида
                        };
                        //добавление в колекцию виды для экспорта
                        options.SetViewsAndSheets(new List<ElementId> { view.Id });
                        
                        // Экспорт изображения
                        docs.ExportImage(options);

                        // Закрытие документа

                        if (docs.ActiveView == null)
                        {
                            docs.Close(false);
                        }
                        
                        Icon fileicon = System.Drawing.Icon.ExtractAssociatedIcon(localFilePath);
                        Bitmap btm = new Bitmap(puthDir+ "\\Ic32 - 3D вид - Вид 1.jpg");
                        imageFamily.Source = Imaging.CreateBitmapSourceFromHBitmap(btm.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        



                    }
                    else
                    {
                        MessageBox.Show("Файл не найден", "Поиск файла", MessageBoxButton.OK, MessageBoxImage.Information);
                        nameFamily.Content = "Наименование семейства";
                        creatureDate.Content = "";
                        weightFile.Content = "";
                        imageFamily.Source = null;
                        /* string errorContent = await response.Content.ReadAsStringAsync();
                         Console.WriteLine($"Ошибка: {response.StatusCode} - {response.ReasonPhrase}");
                         Console.WriteLine($"Содержимое ответа: {errorContent.Substring(0, Math.Min(500, errorContent.Length))}...");
                          File.WriteAllText(debugFilePath, errorContent);*/
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }
        static View3D Find3DView(Document dc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(dc);
            collector.OfClass(typeof(View3D));

            foreach (View3D view in collector)
            {
                if (!view.IsTemplate && view.CanBePrinted)
                {
                    return view;
                }
            }
            return null;
        }
        

        private void openFamily_Click(object sender, RoutedEventArgs e)
        {
            string familyPath =puthDir+"\\"+nameFamily.Content;

            if (string.IsNullOrEmpty(familyPath) || !File.Exists(familyPath))
            {
                MessageBox.Show("Файл не выбран или не существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                UIDocument familyDoc =_uiApp.OpenAndActivateDocument(familyPath);
               
                if (familyDoc != null)
                {
                    
                    MessageBox.Show("Семейство успешно открыто!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось открыть семейство.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
