using System.Net.Http.Headers;
using RoundRobin;

namespace PerfTest;

public static class Requester
{
    public static RoundRobinList<string> RoundRobinList = new RoundRobinList<string>(
        new List<string>{
            "http://127.0.0.1:8080/"
        }
    );
    
    public static HttpClient PrepareHttpClient()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(RoundRobinList.Next()),
            Timeout = TimeSpan.FromSeconds(10)
        };
        httpClient.DefaultRequestHeaders.UserAgent.Clear();

        return httpClient;
    }

    public static async Task<HttpResponseMessage> AddValueRequest(string key, string value)
    {
        var httpClient = PrepareHttpClient();
        
        var request = new HttpRequestMessage(HttpMethod.Post, $"/value/{key}");
        request.Content = new StringContent(value);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        return await httpClient.SendAsync(request);
    }
    
    public static async Task<HttpResponseMessage> RetrieveValueRequest(string key)
    {
        var httpClient = PrepareHttpClient();
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"/value/{key}");
        return await httpClient.SendAsync(request);
    }
}
