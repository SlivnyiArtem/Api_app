using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace API_App
{
    public static class JsonDeserializer
    {
        internal static void DeserializeI(string answer, long id, string savePath)
        {
            var jsonObject = JObject.Parse(answer);
            var title = $"информация о фильме {id}";
            using (StreamWriter w = new StreamWriter(savePath +title + ".txt", false, Encoding.UTF8))
            {
                w.WriteLine(title);
                w.WriteLine(jsonObject.SelectToken("nameRu"));
                w.WriteLine(jsonObject.SelectToken("year"));
                w.WriteLine("Ид Кинопоиска: " + jsonObject.SelectToken("kinopoiskId"));
                w.WriteLine("Жанры: " +  string.Join(", ", jsonObject.SelectToken("genres").Values().Select(el => el.First)));
                w.WriteLine("Ссылка: " + jsonObject.SelectToken("webUrl"));
                w.WriteLine();
                w.WriteLine("Слоган: " + jsonObject.SelectToken("slogan"));
                w.WriteLine();
                w.WriteLine("Сюжет");
                w.WriteLine(jsonObject.SelectToken("description"));
            }
        }

        internal static void DeserializeV(string answer, long id, string savePath)
        {
            var title = $"трейлеры и другие видео о фильме {id}";
            var jsonObject = JObject.Parse(answer);
            var videosArray= jsonObject.SelectToken("items");
            using (StreamWriter w = new StreamWriter(savePath + title + ".txt", false,
                Encoding.UTF8))
            {
                w.WriteLine(title);
                foreach (var video in videosArray.Children())
                {
                    w.WriteLine();
                    w.WriteLine("Ресурс: " + video.SelectToken("site"));
                    w.WriteLine(video.SelectToken("name"));
                    w.WriteLine(video.SelectToken("url"));
                }
            }
        }

        internal static void DeserializeU(string answer, long id, string savePath)
        {
            var jsonArray = JArray.Parse(answer);
            var title = $"информация о сиквелах и приквелах фильма {id}";
            using (StreamWriter w = new StreamWriter(savePath + title + ".txt", false, Encoding.UTF8))
            {
                w.WriteLine(title);
                foreach (var film in jsonArray.Children())
                {
                    w.WriteLine();
                    w.WriteLine(film.SelectToken("nameRu"));
                    w.WriteLine("Id сиквела или приквела: " + film.SelectToken("filmId"));
                    w.WriteLine("Положение на таймлайне: " + film.SelectToken("relationType"));
                }
            }
        }

        internal static void DeserializeS(string answer, long id, string savePath)
        {
            var jsonArray = JArray.Parse(answer);
            var title = $"информация о съемочной группе фильма {id}";
            using (StreamWriter w = new StreamWriter(savePath + title + ".txt", false, Encoding.UTF8))
            {
                w.WriteLine(title);
                foreach (var staffMember in jsonArray.Children())
                {
                    w.WriteLine();
                    w.WriteLine(staffMember.SelectToken("nameRu"));
                    w.WriteLine("Цех: " + staffMember.SelectToken("professionText"));
                    w.WriteLine("Роль: " + staffMember.SelectToken("description"));
                }
            }
        }

        internal static void DeserializeG(string answer, long id, string savePath)
        {
            var jsonObject = JObject.Parse(answer);
            var title = $"информация о фильмах, похожих на {id}";
            using (StreamWriter w = new StreamWriter(savePath + title + ".txt", false, Encoding.UTF8))
            {
                w.WriteLine(title);
                foreach (var film in jsonObject.SelectToken("items").Children())
                {
                    w.WriteLine();
                    w.WriteLine(film.SelectToken("nameRu"));
                    w.WriteLine("Id фильма: " + film.SelectToken("filmId"));
                }
            }
        }
    }
}
