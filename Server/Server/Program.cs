namespace Server;
class Program
{
    static async Task Main(string[] args)
    {
        string[] prefixes = { "http://localhost:8080/" };
        HttpServer httpServer = new HttpServer(prefixes);
        await httpServer.StartAsync();
    }
}
