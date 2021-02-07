namespace YaDiskApiTesting.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Refit;
    using YaDiskApiTesting.Apis;
    using YaDiskApiTesting.Dto;
    using YaDiskApiTesting.Interfaces;

    public class YandexDiskFileUploader
    {
        private readonly IYandexDiskApi _yandexDiskApi;
        private readonly string _oAuthToken;
        private readonly HttpClient _httpClient;

        public YandexDiskFileUploader()
        {
            _yandexDiskApi = RestService.For<IYandexDiskApi>("https://cloud-api.yandex.net");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"config.json", true, true)
                .Build();
            _oAuthToken = "OAuth " + configuration["OAuthToken"];

            _httpClient = new HttpClient();
        }

        public async Task ExecuteAsync(string directoryPath, string destinationPath)
        {
            var files = Directory.GetFiles(directoryPath);

            if (!files.Any())
            {
                Console.WriteLine("No files in directory.");
                return;
            }

            var uploadingTasks = files
                .Select(filePath => UploadAsync(destinationPath, filePath, CancellationToken.None));

            await Task.WhenAll(uploadingTasks);
        }

        private async Task UploadAsync(string destinationPath, string filePath,
            CancellationToken cancellationToken = default)
        {
            var link = await GetUploadLinkAsync($"/{destinationPath}/{Path.GetFileName(filePath)}");

            using (var fileStream = File.OpenRead(filePath))
            {
                var uri = new Uri(link.Href);

                var method = new HttpMethod(link.Method);

                var content = new StreamContent(fileStream);

                var requestMessage = new HttpRequestMessage(method, uri) { Content = content };

                Console.WriteLine($"Started uploading file: {Path.GetFileName(filePath)}");

                try
                {
                    await SendAsync(requestMessage, cancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }

                Console.WriteLine($"{Path.GetFileName(filePath)} successfully uploaded!");
            }
        }

        private async Task<Link> GetUploadLinkAsync(string destinationPath)
        {
            var apiResponse = await _yandexDiskApi.UploadAsync(destinationPath, _oAuthToken, true);

            if (apiResponse.IsSuccessStatusCode)
                return apiResponse.Content;

            var error = JsonConvert.DeserializeObject<ErrorModel>(apiResponse.Error.Content);

            throw new HttpRequestException($"{apiResponse.StatusCode}: {error.Message}");
        }

        private async Task SendAsync([NotNull] HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                response.Content?.Dispose();

                if (response.StatusCode == HttpStatusCode.Unauthorized
                    || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new Exception(response.ReasonPhrase);
                }

                throw new Exception(response.StatusCode + response.ReasonPhrase);
            }
        }
    }
}
