using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MindeeAPI_OCR.Services
{
    public class MindeeApiClient
    {
        private readonly string _apiKey;

        public MindeeApiClient()
        {
            _apiKey = App.Configuration["MindeeApi:ApiKey"];
        }

        public async Task<string> ExtractInvoiceDataAsync(string imagePath)
        {
            var url = "https://api.mindee.net/v1/products/mindee/invoices/v4/predict";
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {_apiKey}");

            var form = new MultipartFormDataContent();
            form.Add(new ByteArrayContent(File.ReadAllBytes(imagePath)), "document", Path.GetFileName(imagePath));

            var response = await httpClient.PostAsync(url, form);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
