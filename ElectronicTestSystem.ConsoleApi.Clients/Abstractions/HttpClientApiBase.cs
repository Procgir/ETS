namespace ElectronicTestSystem.ConsoleApi.Clients.Abstractions;

public abstract class HttpClientApiBase
{
    public string BaseUrl { get; }
    public HttpClient HttpClient { get; }

    public HttpClientApiBase(string baseUrl, HttpClient httpClient)
    {
        BaseUrl = baseUrl;
        HttpClient = httpClient;
    }
}