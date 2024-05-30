using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server
{
    public class BookSearchService
    {
        private string apiUrl = "https://api.nytimes.com/svc/books/v3/reviews.json";
        private string apiKey = "7DQNwuHGWsu87ZfjCmO9Pg4sSzdmDPAs";

        public async Task<List<Book>> SearchBooksByAuthorAsync(string author)
        {
            List<Book> books = new List<Book>();

            string query = $"?author={author}&api-key={apiKey}";
            string fullUrl = apiUrl + query;

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage apiResponse = await httpClient.GetAsync(fullUrl);
                    if (apiResponse.IsSuccessStatusCode)
                    {
                        string responseBody = await apiResponse.Content.ReadAsStringAsync();

                        // Deserializacija JSON objekta
                        JObject jsonData = JObject.Parse(responseBody);

                        // Ekstrakcija liste knjiga iz JSON objekta
                        JArray booksArray = (JArray)jsonData["results"];

                        // Deserializacija liste knjiga
                        List<Book> deserializedBooks = booksArray.ToObject<List<Book>>();

                        foreach (var item in deserializedBooks)
                        {
                            books.Add(new Book(item.Url, item.PublicationDate, item.Title, item.Author, item.Summary));
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Greska sa API-ja: {apiResponse.StatusCode}");
                    }
                }

                return books;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Greska u funkciji SearchBooksByAuthor: {e.Message}");
                throw;
            }
        }
    }
}