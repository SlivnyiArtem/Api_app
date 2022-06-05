using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

namespace API_App
{
    public class ApiApplication
    {
        public Dictionary<string, Func<long, Uri>> RequestsConstructorsDictionary;
        
        private readonly Queue<UserInputData> _queueUserInputData;
        private string SavePath;

        public ApiApplication()
        {
            _queueUserInputData = new Queue<UserInputData>();
            
            var responseBody = File.ReadAllText("..\\..\\..\\UserInput.json", Encoding.UTF8);
            JObject jObject = JObject.Parse(responseBody);
            SavePath = jObject.SelectToken("SavePath").ToObject<string>();
            if (IsUncorrectJson(jObject))
            {
                Console.WriteLine("Недопустимые изменения в структуре UserInput.json");
            }
            var id = TryGetIdFromInput(jObject["Id"]);
            _queueUserInputData.Enqueue(new UserInputData(id,
                jObject["Name"].ToObject<string>(), jObject["Type"].ToObject<string>()));

            RequestsConstructorsDictionary = new Dictionary<string, Func<long, Uri>>
            {
                { "I", RequestTypesFormers.FormRequestFilmIdUrl },
                { "V", RequestTypesFormers.FormRequestTrailersAndVideos },
                { "U", RequestTypesFormers.FormRequestFilmSequelsAndPrequels },
                { "S", RequestTypesFormers.FormRequestFindStaff },
                { "G", RequestTypesFormers.FormRequestFilmSimilar },
            };
        }

        private bool IsUncorrectJson(JObject jObject)
        {
            return (!jObject.ContainsKey("Type") ||
                    !jObject.ContainsKey("SavePath") ||
                    !jObject.ContainsKey("Id") ||
                    !jObject.ContainsKey("Name"));
        }

        private long? TryGetIdFromInput(JToken idObj)
        {
            if (idObj.Type == JTokenType.Undefined)
                return null;
            return idObj.ToObject<long>();
        }


        public void AskApi()
        {
            while (_queueUserInputData.Count > 0)
            {
                try
                {
                    var curInput = _queueUserInputData.Dequeue();
                    if (!curInput.Id.HasValue)
                    {
                        var response = HttpConnectionService.GetAnswer(
                            RequestTypesFormers.FormRequestFindByName(curInput.Name), new HttpClient());
                        var idList = JObject.Parse(response).GetValue("films").Select(el => el.First.ToObject<long>()).ToList();
                        foreach (var id in idList)
                            _queueUserInputData.Enqueue(new UserInputData(id, null, curInput.Type));
                        if (_queueUserInputData.Count == 0)
                            throw new WebException();
                    }

                    else
                    {
                        if (RequestsConstructorsDictionary.ContainsKey(curInput.Type))
                        {
                            var idValue = curInput.Id.Value;
                            var answer =
                                HttpConnectionService.GetAnswer(
                                    RequestsConstructorsDictionary[curInput.Type].Invoke(idValue),
                                    new HttpClient());

                            switch (curInput.Type)
                            {
                                case "I":
                                    JsonDeserializer.DeserializeI(answer, idValue, SavePath);
                                    break;
                                case "V":
                                    JsonDeserializer.DeserializeV(answer, idValue, SavePath);
                                    break;
                                case "U":
                                    JsonDeserializer.DeserializeU(answer, idValue, SavePath);
                                    break;
                                case "S":
                                    JsonDeserializer.DeserializeS(answer, idValue, SavePath);
                                    break;
                                case "G":
                                    JsonDeserializer.DeserializeG(answer, idValue, SavePath);
                                    break;
                                default:
                                    continue;
                            }
                        }
                        else
                            Console.WriteLine("Неизвестный тип запроса к Api");
                    }
                }
                catch (WebException webExc)
                {
                    Console.WriteLine("В процессе работы с сервером произошла ошибка соединения: возможно id или имени фильма, указанного при запросе " +
                                      "не существует в базе данных kinopoisk: " + webExc.Message);
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("Отсутствие ответа от сервера/сервером был возвращен пустой ответ");
                }
            }
        }
    }
}
