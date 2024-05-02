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

        public MindeeApiClient(string apikey)
        {
            _apiKey = apikey;
        }

        public async Task<string> ExtractInvoiceDataAsync(string imagePath)
        {
            var url = "https://api.mindee.net/v1/products/mindee/invoices/v4/predict";
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {_apiKey}");

            try
            {
                var form = new MultipartFormDataContent();
                form.Add(new ByteArrayContent(File.ReadAllBytes(imagePath)), "document", Path.GetFileName(imagePath));

                var response = await httpClient.PostAsync(url, form);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException("Unable to contact the Mindee API", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred processing the image", ex);
            }

            
        }
    }
}
