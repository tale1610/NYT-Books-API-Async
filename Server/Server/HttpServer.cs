using System.Net;
using System.Diagnostics;

namespace Server
{
    public class HttpServer
    {
        private readonly HttpListener listener;

        public HttpServer(string[] prefixes)
        {
            listener = new HttpListener();
            foreach (string prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }
        }

        public async Task StartAsync()
        {
            listener.Start();
            Console.WriteLine("Server pokrenut...");

            while (true)
            {
                var context = await listener.GetContextAsync();
                _ = Task.Run(() => ProcessRequestAsync(context));
            }
        }

        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            Console.WriteLine($"Primljen zahtev: {request.Url}");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                string author = request.QueryString["author"];
                if (string.IsNullOrEmpty(author))
                {
                    SendBadRequestResponse(response);
                    return;
                }

                if (BookCache.ContainsKey(author))
                {
                    await SendCachedResponseAsync(author, response);
                    sw.Stop();
                    Console.WriteLine($"Vreme potrebno za pribavljanje podataka iz kesa za autora {author}: {sw.Elapsed}");
                }
                else
                {
                    await SearchAndSendResponseAsync(author, response);
                    sw.Stop();
                    Console.WriteLine($"Vreme potrebno za pribavljanje podataka koji nije u kesu za autora {author}: {sw.Elapsed}");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Greska u HttpServer: {e.Message}");
                throw;
            }
        }

        private void SendBadRequestResponse(HttpListenerResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.StatusDescription = "Bad Request";
            response.Close();
        }

        private async Task SendCachedResponseAsync(string author, HttpListenerResponse response)
        {
            List<Book> cachedResponse = BookCache.GetValue(author);
            string responseBody = Newtonsoft.Json.JsonConvert.SerializeObject(cachedResponse);
            await SendResponseAsync(response, responseBody);
        }

        private async Task SearchAndSendResponseAsync(string author, HttpListenerResponse response)
        {
            BookSearchService bookSearchService = new BookSearchService();
            List<Book> books = await bookSearchService.SearchBooksByAuthorAsync(author);

            BookCache.Add(author, books);

            string responseBody = Newtonsoft.Json.JsonConvert.SerializeObject(books);
            await SendResponseAsync(response, responseBody);
        }

        private async Task SendResponseAsync(HttpListenerResponse response, string responseBody)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseBody);
            response.ContentLength64 = buffer.Length;
            using (Stream output = response.OutputStream)
            {
                await output.WriteAsync(buffer, 0, buffer.Length);
            }
        }
    }
}