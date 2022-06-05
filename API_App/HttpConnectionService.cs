using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace API_App
{
    public class HttpConnectionService
    {
        private static string key = "513567bd-5022-4783-95e7-d0534e81ac0d";

        public static string GetAnswer(Uri uriRequest, HttpClient httpClient)
        { 
            var request = WebRequest.CreateHttp(uriRequest);
            request.Method = "GET";
            request.Headers.Add("X-API-KEY", key);
            var response = request.GetResponse();
            if (response.GetResponseStream() == null)
                throw new NullReferenceException();
            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }
    }
}
