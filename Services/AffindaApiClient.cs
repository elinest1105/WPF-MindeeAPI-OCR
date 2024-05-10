using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

public class AffindaApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public AffindaApiClient(string apiKey)
    {
        _httpClient = new HttpClient();
        _apiKey = apiKey;
    }

    public async Task<string> ExtractInvoiceData(string imagePath)
    {
        var apiUrl = "https://api.affinda.com/v3/invoice/extract";

        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");

        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(await File.ReadAllBytesAsync(imagePath)), "file", Path.GetFileName(imagePath));
        request.Content = content;

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
