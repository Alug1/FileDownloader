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
        // Укажите ваши данные для подключения к Nextcloud
        string nextcloudUrl = "https://cloud.tavrida.ru/index.php/apps/files/files/2370701?dir=/TER_SubUnit_Support_OHL/%D0%A1%D0%B5%D0%BC%D0%B5%D0%B9%D1%81%D1%82%D0%B2%D0%B0%20%28%D0%B4%D0%B5%D1%82%D0%B0%D0%BB%D0%B8%29";
        string username = "lugas@tavrida.ru";
        string password = "Qwerty12345";
        string fileName = "Болт.rfa"; // Имя файла, который нужно скачать
        string localFilePath = Path.Combine(Directory.GetCurrentDirectory(), fileName); // Путь для сохранения файла
                                                                                        // Создаем HttpClient
        using (HttpClient client = new HttpClient())
        {
            // Добавляем заголовок авторизации
            var authToken = Encoding.ASCII.GetBytes($"{username}:{password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

            // Выполняем GET-запрос для скачивания файла
            HttpResponseMessage response = await client.GetAsync(nextcloudUrl, HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
            {
                // Читаем содержимое файла как поток
                using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                              fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                {
                    // Копируем поток в файл
                    await contentStream.CopyToAsync(fileStream);
                }

                Console.WriteLine($"Файл '{fileName}' успешно скачан и сохранен в '{localFilePath}'.");
            }
            else
            {
                Console.WriteLine($"Ошибка: {response.StatusCode}");
            }
        }
    }
}
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