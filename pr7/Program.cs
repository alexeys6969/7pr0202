using HtmlAgilityPack;
using System;
using System.Collections;
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
            Debug.Listeners.Add(new TextWriterTraceListener("debug.log"));
            Debug.AutoFlush = true;

            Debug.WriteLine($"=== Запуск программы: {DateTime.Now} ===");

            CookieContainer Token = SignIn("user", "user");
            string html = GetContent(Token);
            ParsingHtml(html);

            Debug.WriteLine("=== Завершение работы ===");
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

        public static void ParsingHtml(string htmlCode)
        {
            var html = new HtmlDocument();
            html.LoadHtml(htmlCode);
            var Document = html.DocumentNode;
            IEnumerable DivsNews = Document.Descendants(0).Where(n => n.HasClass("news"));
            foreach (HtmlNode DivNews in DivsNews)
            {
                var src = DivNews.ChildNodes[1].GetAttributeValue("src", "none");
                var name = DivNews.ChildNodes[3].InnerText;
                var description = DivNews.ChildNodes[5].InnerText;
                Console.WriteLine(name + "\n" + "Изображение: " + src + "\n" + "Описание: " + description + "\n");
            }
        }
    }
}
