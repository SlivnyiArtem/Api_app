using System;

namespace API_App
{
    public static class RequestTypesFormers
    {
        private static string GetUriCore(string version) => $"https://kinopoiskapiunofficial.tech/api/{version}/";
        public static Uri FormRequestFilmIdUrl(long id) => new Uri($"{GetUriCore("v2.2")}films/{id}");

        public static Uri FormRequestFilmSimilar(long id) => new Uri($"{GetUriCore("v2.2")}films/{id}/similars");

        public static Uri FormRequestFilmSequelsAndPrequels(long id) => new Uri($"{GetUriCore("v2.1")}films/{id}/sequels_and_prequels");

        public static Uri FormRequestTrailersAndVideos(long id) => new Uri($"{GetUriCore("v2.2")}films/{id}/videos");

        public static Uri FormRequestFindStaff(long id) => new Uri($"{GetUriCore("v1")}staff?filmId={id}");

        public static Uri FormRequestFindByName(string name) =>
            new Uri($"{GetUriCore("v2.1")}films/search-by-keyword?keyword={name}&page=1");
    }
}