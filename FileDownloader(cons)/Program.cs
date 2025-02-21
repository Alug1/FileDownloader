using System;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


class Program
{
    static async Task Main(string[] args)
    {
        // Укажите данные для подключения к Nextcloud
        string nextcloudBaseUrl = "https://cloud.tavrida.ru/remote.php/dav/files/lugas@tavrida.ru/"; // WebDAV базовый URL
        string filePath = "TER_SubUnit_Support_OHL/Семейства (детали)/TER_SubComp_Bracket_SK(7-1A).rfa"; // Путь к файлу
        string username = "lugas@tavrida.ru";
        string password = "Qwerty12345";
        string localFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TER_SubComp_Bracket_SK(7-1A).rfa");
        string debugFilePath = Path.Combine(Directory.GetCurrentDirectory(), "debug_response.html"); // Для отладки


        // Формируем полный URL
        string fullUrl = "https://cloud.tavrida.ru/index.php/apps/files/files/2370701?dir=/TER_SubUnit_Support_OHL/%D0%A1%D0%B5%D0%BC%D0%B5%D0%B9%D1%81%D1%82%D0%B2%D0%B0%20%28%D0%B4%D0%B5%D1%82%D0%B0%D0%BB%D0%B8%29";
        Console.WriteLine($"Запрос отправляется на: {fullUrl}"); // Для отладки

        // Создаем HttpClient
        using (HttpClient client = new HttpClient())
        {
            // Добавляем заголовок авторизации
            var authToken = Encoding.ASCII.GetBytes($"{username}:{password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

            try
            {
                // Выполняем GET-запрос
                HttpResponseMessage response = await client.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Читаем содержимое как поток
                    using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }

                    // Сохраняем содержимое как текст для отладки
                    string responseContent = await response.Content.ReadAsStringAsync();
                    await File.WriteAllTextAsync(debugFilePath, responseContent);

                    Console.WriteLine($"Файл сохранен как '{localFilePath}', размер: {new FileInfo(localFilePath).Length} байт");
                    Console.WriteLine($"Ответ сервера сохранен в '{debugFilePath}' для анализа");
                }
                else
                {
                    Console.WriteLine($"Ошибка: {response.StatusCode} - {response.ReasonPhrase}");
                    string errorContent = await response.Content.ReadAsStringAsync();
                    await File.WriteAllTextAsync(debugFilePath, errorContent);
                    Console.WriteLine($"Ответ сервера сохранен в '{debugFilePath}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }
    }
}
//class Program
//{
//    static async Task Main(string[] args)
//    {
//        // Укажите ваши данные для подключения к Nextcloud
//        string nextcloudUrl = "https://cloud.tavrida.ru/index.php/apps/files/files/2370701?dir=/TER_SubUnit_Support_OHL/%D0%A1%D0%B5%D0%BC%D0%B5%D0%B9%D1%81%D1%82%D0%B2%D0%B0%20%28%D0%B4%D0%B5%D1%82%D0%B0%D0%BB%D0%B8%29";
//        string username = "lugas@tavrida.ru";
//        string password = "Qwerty12345";
//        string fileName = "Болт.rfa"; // Имя файла, который нужно скачать
//        string localFilePath = Path.Combine(Directory.GetCurrentDirectory(), fileName); // Путь для сохранения файла

//        // Создаем HttpClient                                                                          
//        using (HttpClient client = new HttpClient())
//        {
//            // Устанавливаем базовый адрес
//            client.BaseAddress = new Uri(nextcloudUrl);

//            // Добавляем заголовок авторизации
//            var authToken = Encoding.ASCII.GetBytes($"{username}:{password}");
//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

//            // Выполняем GET-запрос для скачивания файла
//            HttpResponseMessage response = await client.GetAsync(fileName);

//            if (response.IsSuccessStatusCode)
//            {
//                // Открываем поток для сохранения файла
//                using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
//                {
//                    // Копируем содержимое ответа прямо в файл
//                    await response.Content.CopyToAsync(fileStream);
//                }

//                Console.WriteLine($"Файл '{fileName}' успешно скачан и сохранен в '{localFilePath}'.");
//            }
//            else
//            {
//                Console.WriteLine($"Ошибка: {response.StatusCode}");
//            }
//        }
//    }
//}



//        // Создаем HttpClient
//        using (HttpClient client = new HttpClient())
//        {
//            // Устанавливаем базовый адрес
//            client.BaseAddress = new Uri(nextcloudUrl);

//            // Добавляем заголовок авторизации
//            var authToken = Encoding.ASCII.GetBytes($"{username}:{password}");
//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

//            // Выполняем GET-запрос для скачивания файла
//            HttpResponseMessage response = await client.GetAsync(fileName);

//            if (response.IsSuccessStatusCode)
//            {
//                // Читаем содержимое файла
//                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();

//                // Сохраняем файл на диск
//                File.WriteAllBytesAsync(localFilePath, fileBytes);

//                Console.WriteLine($"Файл '{fileName}' успешно скачан и сохранен в '{localFilePath}'.");
//            }
//            else
//            {
//                Console.WriteLine($"Ошибка: {response.StatusCode}");
//            }
//        }
//    }
//}

//        // Создаем URI для файла
//        string fileUrl = nextcloudUrl + fileName;

//        // Игнорируем ошибки SSL (если используется самоподписанный сертификат)
//        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

//        // Создаем WebClient и добавляем авторизацию
//        using (WebClient client = new WebClient())
//        {
//            // Добавляем заголовок авторизации
//            string authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
//            client.Headers[HttpRequestHeader.Authorization] = "Basic " + authToken;

//            try
//            {
//                // Скачиваем файл
//                client.DownloadFile(fileUrl, localFilePath);
//                Console.WriteLine($"Файл '{fileName}' успешно скачан и сохранен в '{localFilePath}'.");
//            }
//            catch (WebException ex)
//            {
//                // Обработка ошибок
//                Console.WriteLine($"Ошибка при скачивании файла: {ex.Message}");
//                if (ex.Response != null)
//                {
//                    using (var stream = ex.Response.GetResponseStream())
//                    using (var reader = new StreamReader(stream))
//                    {
//                        Console.WriteLine(reader.ReadToEnd());
//                    }
//                }
//            }
//        }
//    }
//}




//Отсыпка вокруг опоры.rfa
//lugas@tavrida.ru
//Qwerty12345
//https://cloud.tavrida.ru/index.php/apps/files/files/2370701?dir=/TER_SubUnit_Support_OHL/%D0%A1%D0%B5%D0%BC%D0%B5%D0%B9%D1%81%D1%82%D0%B2%D0%B0%20%28%D0%B4%D0%B5%D1%82%D0%B0%D0%BB%D0%B8%29