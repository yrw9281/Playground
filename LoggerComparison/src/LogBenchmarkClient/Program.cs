using System.Diagnostics;
using System.Net.Http.Json;

const string serilogUrl = "http://localhost:5001/log";
const string nlogUrl = "http://localhost:5002/log";
const int iterations = 10;
const int logsPerRequest = 100;

var client = new HttpClient();

Console.WriteLine($"Starting benchmark: {iterations} iterations, {logsPerRequest} logs each");

await RunBenchmark("Serilog", serilogUrl);
await RunBenchmark("NLog", nlogUrl);

async Task RunBenchmark(string label, string url)
{
    Console.WriteLine($"\n== {label} Benchmark ==");

    long totalElapsed = 0;

    for (int i = 1; i <= iterations; i++)
    {
        var payload = new LogRequest($"{label} test log entry", logsPerRequest);
        var sw = Stopwatch.StartNew();
        var response = await client.PostAsJsonAsync(url, payload);
        sw.Stop();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Iteration {i}: Failed with status {response.StatusCode}");
            continue;
        }

        var result = await response.Content.ReadFromJsonAsync<LogResponse>();
        Console.WriteLine($"Iteration {i}: API elapsed {result?.ElapsedMilliseconds}ms, Client total {sw.ElapsedMilliseconds}ms");

        totalElapsed += sw.ElapsedMilliseconds;
    }

    Console.WriteLine($"\n{label} Average Elapsed Time: {totalElapsed / iterations}ms");
}

record LogRequest(string Message, int Count);
record LogResponse(int Count, string Message, long ElapsedMilliseconds);
