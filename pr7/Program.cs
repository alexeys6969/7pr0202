using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace pr7
{
    public class Program
    {
        static void Main(string[] args)
        {
            CookieContainer Token = SignIn("user", "user");
            GetContent(Token);
            Console.Read();
        }

        public static CookieContainer SignIn(string Login, string Password)
        {
            string url = "http://localhost/news.permaviat.ru/ajax/login.php";
            Debug.WriteLine($"Выполняем запрос: {url}");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            CookieContainer cookies = new CookieContainer();
            request.CookieContainer = cookies;

            string postData = $"login={Login}&password={Password}";
            byte[] Data = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = Data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(Data, 0, Data.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Debug.WriteLine($"Статус выполнения: {response.StatusCode}");
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseFromServer = reader.ReadToEnd();
                    Console.WriteLine(responseFromServer);
                }
            }

            return cookies;
        }

        public static string GetContent(CookieContainer cookies)
        {
            string url = "http://localhost/news.permaviat.ru/main";
            Debug.WriteLine($"Выполняем запрос: {url}");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = cookies;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Debug.WriteLine($"Статус выполнения: {response.StatusCode}");
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseFromServer = reader.ReadToEnd();
                    return responseFromServer;
                }
            }
        }
    }
}
