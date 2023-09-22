using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpDataCommandClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpDataCommandClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration =  configuration;
        }
        public async Task SendPlatformToCommand(PlatformReadDtos platformReadDtos)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(platformReadDtos),
                Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Sync POST to Command service was OK");
            }
            else
            {
                Console.WriteLine("--> Sync POST to Command service was NOT OK");

            }
        }
    }
}