using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Parser_Kinopoisk_AllReviews
{
    public class Country
    {
        public string country { get; set; }
    }

    public class Genre
    {
        public string genre { get; set; }
    }

    public class Film
    {
        public int filmId { get; set; }
        public string nameRu { get; set; }
        public string nameEn { get; set; }
        public string type { get; set; }
        public string year { get; set; }
        public string description { get; set; }
        public string filmLength { get; set; }
        public List<Country> countries { get; set; }
        public List<Genre> genres { get; set; }
        public string rating { get; set; }
        public int ratingVoteCount { get; set; }
        public string posterUrl { get; set; }
        public string posterUrlPreview { get; set; }
    }

    public class Root
    {
        public string keyword { get; set; }
        public int pagesCount { get; set; }
        public List<Film> films { get; set; }
        public int searchFilmsCountResult { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Review
    {
        public int reviewId { get; set; }
        public string reviewType { get; set; }
        public DateTime reviewData { get; set; }
        public int userPositiveRating { get; set; }
        public int userNegativeRating { get; set; }
        public string reviewAutor { get; set; }
        public string reviewTitle { get; set; }
        public string reviewDescription { get; set; }
    }

    public class Root2
    {
        public int page { get; set; }
        public int filmId { get; set; }
        public int reviewAllCount { get; set; }
        public object reviewAllPositiveRatio { get; set; }
        public object reviewPositiveCount { get; set; }
        public object reviewNegativeCount { get; set; }
        public object reviewNeutralCount { get; set; }
        public int pagesCount { get; set; }
        public List<Review> reviews { get; set; }
    }

    class Program
    { 

        static string getFilmIdByFilmName(string filmName)
        {
            WebRequest request = WebRequest.Create("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-keyword?keyword=" + filmName + "&page=1");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("X-API-KEY", "6b82396f-93f4-4bb7-86e1-571960f22be0");
            WebResponse response = request.GetResponse();
            string answer = "";
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        answer += line;
                    }
                }
            }
            response.Close();
            Root datalist = JsonConvert.DeserializeObject<Root>(answer);
            string filmId = datalist.films[0].filmId.ToString();
            return filmId;
        }

        static List<string> getCommentsByFilmId(string filmId, bool param)
        {
            int pagesCounter = 1, allPages = 1;
            List<string> comments = new List<string>();
            do
            {
                WebRequest request = WebRequest.Create("https://kinopoiskapiunofficial.tech/api/v1/reviews?filmId=" + filmId + "&page=" + pagesCounter);
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("X-API-KEY", "6b82396f-93f4-4bb7-86e1-571960f22be0");
                WebResponse response = request.GetResponse();
                string answer = "";
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line = "";
                        while ((line = reader.ReadLine()) != null)
                        {
                            answer += line;
                        }
                    }
                }
                Root2 datalist = JsonConvert.DeserializeObject<Root2>(answer);
                allPages = datalist.pagesCount;
                foreach(Review rev in datalist.reviews)
                {
                    if (param)
                        comments.Add(rev.reviewDescription);
                    else
                    {
                        if (rev.reviewType == "POSITIVE" || rev.reviewType == "NEGATIVE")
                            comments.Add(rev.reviewDescription);
                    }
                }
                pagesCounter++;
                response.Close();
            } while (pagesCounter <= allPages);
            return comments;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Введите название фильма: ");
            string filmName = Console.ReadLine();
            string filmId = getFilmIdByFilmName(filmName);
            Console.WriteLine("ID: " + filmId);
            List<string> comments = getCommentsByFilmId(filmId, false);
            Console.WriteLine(comments.Count);
            /*            foreach (string comm in comments)
                            Console.WriteLine(comm);*/
        }
    }
}

