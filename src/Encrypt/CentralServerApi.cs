using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class CentralServiceOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string InquiryAndRecordUrl { get; set; } = string.Empty;
    public string ConfirmUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
}

public class CentralServiceApi
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = false,
    };

    public readonly ILogger<CentralServiceApi> _logger;
    public readonly IOptions<CentralServiceOptions> _options;
    public readonly HttpClient _httpClient;

    public CentralServiceApi(
        ILogger<CentralServiceApi> logger,
        IOptions<CentralServiceOptions> options,
        HttpClient client)
    {
        _logger = logger;
        _options = options;
        _httpClient = client;
        _httpClient.DefaultRequestHeaders.Add("x-api-key", _options.Value.ApiKey);
    }

    public async Task InquiryAndRecordAsync(InquiryAndRecordRequest requestData, int testNo)
    {
        var requestJson = JsonSerializer.Serialize<InquiryAndRecordRequest>(requestData, _jsonSerializerOptions);
        await ExecuteApi(_options.Value.InquiryAndRecordUrl, requestJson, testNo);
    }

    public async Task Confirm(ConfirmRequest requestData, int testNo)
    {
        var requestJson = JsonSerializer.Serialize<ConfirmRequest>(requestData, _jsonSerializerOptions);
        await ExecuteApi(_options.Value.ConfirmUrl, requestJson, testNo);
    }

    public async Task Cancel(CancelRequest requestData, int testNo)
    {
        var requestJson = JsonSerializer.Serialize<CancelRequest>(requestData, _jsonSerializerOptions);
        await ExecuteApi(_options.Value.CancelUrl, requestJson, testNo);
    }

    private async Task ExecuteApi(string url, string requestJson, int testNo)
    {
        WriteToRequestFile(requestJson, testNo);

        var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        _logger.LogInformation($"Response StatusCode: {response.StatusCode}");

        var resContent = await response.Content.ReadAsStringAsync();
        WriteToResponseFile(resContent, testNo);
    }

    private void WriteToRequestFile(string requestJson, int testNo)
    {
        using (var file = new StreamWriter(Path.Join("Data", $"NO_{testNo,2:00}.json")))
        {
            file.Write(requestJson);
        }

        _logger.LogDebug($"No.{testNo,2:00} Request: {requestJson}");
    }

    private void WriteToResponseFile(string responseJson, int testNo)
    {
        using (var file = new StreamWriter(Path.Join(Path.Join("Data", "Response"), $"NO_{testNo,2:00}.json")))
        {
            file.Write(responseJson);
        }

        _logger.LogDebug($"No.{testNo,2:00} Response: {responseJson}");
    }
}