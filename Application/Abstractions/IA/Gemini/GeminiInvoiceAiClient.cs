// using System.Net.Http.Json;
// using System.Text.Json;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
//
// namespace Application.Abstractions.IA.Gemini;
//
// public sealed class GeminiInvoiceAiClient : IInvoiceAiClient
// {
//     private readonly HttpClient _httpClient;
//     private readonly ILogger<GeminiInvoiceAiClient> _logger;
//
//     private const string GeminiEndpoint =
//         "https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent";
//
//     public GeminiInvoiceAiClient(
//         HttpClient httpClient,
//         IConfiguration configuration,
//         ILogger<GeminiInvoiceAiClient> logger)
//     {
//         _httpClient = httpClient;
//         _logger = logger;
//
//         var apiKey = configuration["Gemini:ApiKey"];
//         if (string.IsNullOrWhiteSpace(apiKey))
//             throw new ArgumentException("Gemini ApiKey not configured");
//
//         if (!_httpClient.DefaultRequestHeaders.Contains("x-goog-api-key"))
//         {
//             _httpClient.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);
//         }
//     }
//
//     public async Task<IReadOnlyList<AiInvoiceExtractionItem>> ExtractInvoiceAsync(
//         AiInvoiceExtractionRequest request,
//         CancellationToken cancellationToken = default)
//     {
//         try
//         {
//             var requestBody = new
//             {
//                 contents = new[]
//                 {
//                     new
//                     {
//                         role = "user",
//                         parts = new object[]
//                         {
//                             new { text = request.Prompt },
//                             new
//                             {
//                                 inline_data = new
//                                 {
//                                     mime_type = request.MimeType,
//                                     data = request.Base64File
//                                 }
//                             }
//                         }
//                     }
//                 }
//             };
//
//             var response = await _httpClient.PostAsJsonAsync(
//                 GeminiEndpoint,
//                 requestBody,
//                 cancellationToken);
//
//             if (!response.IsSuccessStatusCode)
//             {
//                 var error = await response.Content.ReadAsStringAsync(cancellationToken);
//                 _logger.LogError(
//                     "Gemini API error | Status: {Status} | Body: {Body}",
//                     response.StatusCode,
//                     error);
//
//                 return Array.Empty<AiInvoiceExtractionItem>();
//             }
//
//             var rawResponse = await response.Content.ReadAsStringAsync(cancellationToken);
//             return ParseResponse(rawResponse);
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Unexpected error calling Gemini API");
//             return Array.Empty<AiInvoiceExtractionItem>();
//         }
//     }
//
//     private IReadOnlyList<AiInvoiceExtractionItem> ParseResponse(string response)
//     {
//         try
//         {
//             using var doc = JsonDocument.Parse(response);
//
//             var text = doc.RootElement
//                 .GetProperty("candidates")[0]
//                 .GetProperty("content")
//                 .GetProperty("parts")[0]
//                 .GetProperty("text")
//                 .GetString();
//
//             if (string.IsNullOrWhiteSpace(text))
//                 return Array.Empty<AiInvoiceExtractionItem>();
//
//             var cleanJson = text
//                 .Replace("```json", "")
//                 .Replace("```", "")
//                 .Trim();
//
//             return JsonSerializer.Deserialize<List<AiInvoiceExtractionItem>>(
//                        cleanJson,
//                        new JsonSerializerOptions
//                        {
//                            PropertyNameCaseInsensitive = true
//                        })
//                    ?? Array.Empty<AiInvoiceExtractionItem>();
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error parsing Gemini response JSON");
//             return Array.Empty<AiInvoiceExtractionItem>();
//         }
//     }
// }
