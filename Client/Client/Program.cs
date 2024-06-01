using System;
using System.Collections.Generic;
using System.Net.Http;

class Program
{
    static async Task Main(string[] args)
    {
        for (int i = 0; i < 3; i++)
        {
            // Kreiranje liste autora za pretragu
            List<string> authors = new List<string> { "Stephen King", "J.K. Rowling", "George Orwell" };

            // Kreiranje liste taskova za svakog autora
            List<Task> tasks = new List<Task>();
            foreach (var author in authors)
            {
                tasks.Add(ProcessAuthorAsync(author));
            }

            // Čekanje da se svi taskovi završe
            await Task.WhenAll(tasks);

            Console.WriteLine("Odgovoreno na sve zahteve.");
        }
    }

    static async Task ProcessAuthorAsync(string author)
    {
        string serverUrl = $"http://localhost:8080/?author={author}";

        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(serverUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
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
    }
}
