namespace Server;
class Program
{
    static void Main(string[] args)
    {
        string[] prefixes = { "http://localhost:8080/" };
        HttpServer httpServer = new HttpServer(prefixes);
        httpServer.Start();
    }
}
