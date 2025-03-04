using System;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections.Specialized;

class Program
{
    static async Task Main(string[] args)
    {
        Console.Write("Введите логин: ");
        string username = Console.ReadLine();
        Console.Write("Введите пароль: ");
        string password = Console.ReadLine();

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

                if (string.IsNullOrEmpty(requestToken))
                {
                    Console.WriteLine("Не удалось найти requesttoken.");
                    Console.WriteLine($"Содержимое ответа: {initialHtml.Substring(0, Math.Min(500, initialHtml.Length))}...");
                    return;
                }
                Console.WriteLine($"RequestToken: {requestToken}");

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
                        Console.WriteLine($"GUID пользователя {username}: {guid}");
                    }
                    else
                    {
                        Console.WriteLine("GUID не найден в HTML.");
                        Console.WriteLine($"Содержимое главной страницы: {homeHtml.Substring(0, Math.Min(500, homeHtml.Length))}...");
                    }
                }
                else
                {
                    Console.WriteLine($"Ошибка авторизации: {loginResponse.StatusCode} - {loginResponse.ReasonPhrase}");
                    Console.WriteLine($"Содержимое ответа: {loginHtml.Substring(0, Math.Min(500, loginHtml.Length))}...");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
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
}

//class Program
//{
//    static async Task Main(string[] args)
//    {
//        // Точный URL для скачивания файла через WebDAV
//        string downloadUrl = "https://cloud.tavrida.ru/remote.php/dav/files/2650F160-DA99-48BE-8EE7-356D9565A361/TER_SubUnit_Support_OHL/%D0%A1%D0%B5%D0%BC%D0%B5%D0%B9%D1%81%D1%82%D0%B2%D0%B0%20(%D0%B4%D0%B5%D1%82%D0%B0%D0%BB%D0%B8)/TER_SubComp_Holder_PO-2_mod1.rfa";
//        string username = "lugas@tavrida.ru"; // Логин пользователя
//        string password = "Qwerty12345";      // Пароль пользователя
//        string familyName = string.Empty;
//        string localFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TER_SubComp_Bracket_SK(7-1A).rfa");
//        string debugFilePath = Path.Combine(Directory.GetCurrentDirectory(), "debug_response.html");

//        Console.WriteLine($"Запрос отправляется на: {downloadUrl}");

//        // Создаем HttpClient
//        using (HttpClient client = new HttpClient())
//        {
//            // Устанавливаем авторизацию (Basic Auth)
//            var authToken = Encoding.ASCII.GetBytes($"{username}:{password}");
//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

//            try
//            {
//                // Выполняем GET-запрос
//                HttpResponseMessage response = await client.GetAsync(downloadUrl);

//                if (response.IsSuccessStatusCode)
//                {
//                    // Сохраняем файл
//                    using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
//                    {
//                        await response.Content.CopyToAsync(fileStream);
//                    }
//                    Console.WriteLine($"Файл сохранен как '{localFilePath}', размер: {new FileInfo(localFilePath).Length} байт");
//                }
//                else
//                {
//                    string errorContent = await response.Content.ReadAsStringAsync();
//                    Console.WriteLine($"Ошибка: {response.StatusCode} - {response.ReasonPhrase}");
//                    Console.WriteLine($"Содержимое ответа: {errorContent.Substring(0, Math.Min(500, errorContent.Length))}...");
//                    await File.WriteAllTextAsync(debugFilePath, errorContent);
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Ошибка: {ex.Message}");
//            }
//        }
//    }
//}
//https://learn.microsoft.com/ru-ru/dotnet/fundamentals/networking/sockets/socket-services






//Отсыпка вокруг опоры.rfa
//lugas@tavrida.ru
//Qwerty12345
//https://cloud.tavrida.ru/index.php/apps/files/files/2370701?dir=/TER_SubUnit_Support_OHL/%D0%A1%D0%B5%D0%BC%D0%B5%D0%B9%D1%81%D1%82%D0%B2%D0%B0%20%28%D0%B4%D0%B5%D1%82%D0%B0%D0%BB%D0%B8%29