class Program
{
    static void Main(string[] args)
    {
        for (int i = 0; i < 3; i++)
        {
            // Kreiranje liste autora za pretragu
            List<string> authors = new List<string> { "Stephen King", "J.K. Rowling", "George Orwell" };

            // Kreiranje niti za svakog autora
            List<Thread> threads = new List<Thread>();
            foreach (var author in authors)
            {
                Thread thread = new Thread(() =>
                {
                    string serverUrl = $"http://localhost:8080/?author={author}";

                    using (HttpClient httpClient = new HttpClient())
                    {
                        try
                        {
                            HttpResponseMessage response = httpClient.GetAsync(serverUrl).Result;

                            if (response.IsSuccessStatusCode)
                            {
                                string responseBody = response.Content.ReadAsStringAsync().Result;
                                Console.WriteLine($"Odgovor za autora: '{author}':");
                                Console.WriteLine(responseBody);
                            }
                            else
                            {
                                Console.WriteLine($"Greska na serveru za autora: '{author}': {response.StatusCode}");
                            }
                        }
                        catch (HttpRequestException e)
                        {
                            Console.WriteLine($"HTTP request greska za autora: '{author}': {e.Message}");
                        }
                    }
                });

                threads.Add(thread);
                
            }

            // Pokretanje svih niti
            foreach (var thread in threads)
            {
                thread.Start();
            }

            // Čekanje da se sve niti završe
            foreach (var thread in threads)
            {
                thread.Join();
            }

            Console.WriteLine("Odgovoreno na sve zahteve.");
        }
    }
}
