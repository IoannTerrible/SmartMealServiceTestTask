using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using SmsClientLibrary.Common.Clients;
using SmsClientLibrary.Common.Models;

namespace SmsClientLibrary.Http.Clients;

/// <summary>Клиент для отправки HTTP запросов.</summary>
internal class SmsHttpClient : ISmsClient
{
    private readonly HttpClient _httpClient;
    private readonly string _url;

    public SmsHttpClient(string url, string username, string password)
    {
        _url = url;
        _httpClient = new HttpClient();
        var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
    }

    public async Task<GetMenuResponce> GetMenuAsync()
    {
        var request = new
        {
            Command = "GetMenu",
            CommandParameters = new { WithPrice = true }
        };

        var httpResponse = await SendRequestAsync<HttpGetMenuResponse>(request);

        var result = new GetMenuResponce
        {
            Success = httpResponse.Success,
            ErrorMessage = httpResponse.ErrorMessage
        };

        if (httpResponse.Success && httpResponse.Data?.MenuItems != null)
        {
            result.Dishes = httpResponse.Data.MenuItems;
        }

        return result;
    }

    public async Task<SendOrderResponce> SendOrderAsync(Order order)
    {
        var request = new
        {
            Command = "SendOrder",
            CommandParameters = order
        };

        var httpResponse = await SendRequestAsync<HttpSendOrderResponse>(request);

        return new SendOrderResponce
        {
            Success = httpResponse.Success,
            ErrorMessage = httpResponse.ErrorMessage
        };
    }

    private async Task<T> SendRequestAsync<T>(object content)
    {
        var json = JsonSerializer.Serialize(content);
        using var resp = await _httpClient.PostAsync(_url, new StringContent(json, Encoding.UTF8, "application/json"));
        resp.EnsureSuccessStatusCode();
        var respJson = await resp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(respJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    private class HttpGetMenuResponse
    {
        public bool Success { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        public MenuData? Data { get; set; }
    }

    private class MenuData
    {
        public List<Dish> MenuItems { get; set; } = [];
    }

    private class HttpSendOrderResponse
    {
        public bool Success { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;
    }
}
