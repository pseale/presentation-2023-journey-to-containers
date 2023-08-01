// See https://aka.ms/new-console-template for more information
using System.Collections.Concurrent;


var tester = new Tester();
await tester.RunAsync(args);

public class Tester
{
    const int SlowResponseSeconds = 4;

    public async Task RunAsync(string[] args)
    {
        if (args.Length != 2)
            ShowHelpAndExit();

        if (args[0].Trim().ToLowerInvariant() != "--test")
            ShowHelpAndExit();

        if (!(args[1].Trim().ToLowerInvariant() is "good" or "bad"))
            ShowHelpAndExit();

        // we have hardcoded the 'good' website to http://localhost/good and
        // bad website to http://localhost/bad
        var url = $"http://localhost/{args[1].Trim()}/slow?seconds={SlowResponseSeconds}";

        for (int i = 0; i < 20; i++)
        {
            await Task.Delay(200);
            var _ = RepeatedlyPingWebsite(url);
        }

        await WriteToConsole();
    }

    private void ShowHelpAndExit()
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Error.WriteLine("ERROR: invalid arguments provided.");
        Console.ResetColor();
        Console.WriteLine("""
            Usage:
                ConsoleApp1 --test good
              or
                ConsoleApp1 --test bad
            """);
        Environment.Exit(1);
    }

    private ConcurrentQueue<Response> _results = new ConcurrentQueue<Response>();

    private async Task WriteToConsole()
    {
        // repeatedly check the _results Queue and print out any pending results
        while (true)
        {
            while (_results.TryDequeue(out var result))
            {
                Console.ForegroundColor = PrettyPrint(result);
                Console.Write("█");
                Console.ResetColor();
            }
            await Task.Delay(5);
        }
    }

    private ConsoleColor PrettyPrint(Response result)
    {
        if (result == Response.Ok)
            return ConsoleColor.Cyan;
        else if (result == Response.Error502)
            return ConsoleColor.Red;
        else if (result == Response.Error503)
            return ConsoleColor.DarkRed;
        else if (result == Response.Error504)
            return ConsoleColor.Magenta;
        else if (result == Response.Timeout)
            return ConsoleColor.DarkGray;
        else if (result == Response.OtherError)
            return ConsoleColor.Black;
        else
            throw new NotImplementedException($"{result} PrettyPrint not implemented");
    }

    private async Task RepeatedlyPingWebsite(string url)
    {
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(SlowResponseSeconds + 2);

        while (true)
        {
            try
            {
                var response = await httpClient.GetAsync(url);
                _results.Enqueue(Interpret(response));
            }
            catch (TaskCanceledException)
            {
                _results.Enqueue(Response.Timeout);
            }
            catch (Exception)
            {
                _results.Enqueue(Response.OtherError);
            }
        }
    }

    private Response Interpret(HttpResponseMessage response)
    {
        if ((int)response.StatusCode < 300)
            return Response.Ok;

        if ((int)response.StatusCode == 502)
            return Response.Error502;
        if ((int)response.StatusCode == 503)
            return Response.Error503;
        if ((int)response.StatusCode == 504)
            return Response.Error504;

        throw new NotImplementedException();
    }

    public enum Response
    {
        Ok,
        Error502,
        Error503,
        Error504,
        Timeout,
        OtherError
    }
}
