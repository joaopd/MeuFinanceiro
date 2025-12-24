// using System.Net.Http.Json;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using Domain.Abstractions.IA;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
//
// namespace Infra.IA.Gemini;
//
// public sealed class GeminiInvoiceAiClient(
//     HttpClient httpClient,
//     IConfiguration config,
//     ILogger<GeminiInvoiceAiClient> logger)
//     : IInvoiceAiClient
// {
//     public async Task<IReadOnlyList<AiInvoiceExtractionItem>> ExtractInvoiceAsync(
//         AiInvoiceExtractionRequest request, 
//         CancellationToken cancellationToken = default)
//     {
//         var apiKey = config["Gemini:ApiKey"];
//         if (string.IsNullOrEmpty(apiKey))
//             throw new InvalidOperationException("Gemini API Key não configurada.");
//
//         // 1. Converter Stream para Base64
//         using var memoryStream = new MemoryStream();
//         await request.FileStream.CopyToAsync(memoryStream, cancellationToken);
//         var base64Data = Convert.ToBase64String(memoryStream.ToArray());
//
//         // 2. Construir o Prompt (Regras de Negócio de Extração)
//         var prompt = BuildPrompt(request.CategoryContext, request.FileName);
//
//         // 3. Montar o Request Body do Gemini
//         var requestBody = new
//         {
//             contents = new[]
//             {
//                 new
//                 {
//                     role = "user",
//                     parts = new object[]
//                     {
//                         new { text = prompt },
//                         new
//                         {
//                             inline_data = new
//                             {
//                                 mime_type = request.MimeType,
//                                 data = base64Data
//                             }
//                         }
//                     }
//                 }
//             }
//         };
//
//         // 4. Executar a chamada HTTP
//         // Nota: O modelo 'gemini-1.5-flash' é mais rápido e barato para documentos
//         var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";
//         
//         var response = await httpClient.PostAsJsonAsync(url, requestBody, cancellationToken);
//
//         if (!response.IsSuccessStatusCode)
//         {
//             var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
//             logger.LogError("Erro na API do Gemini: {StatusCode} - {Content}", response.StatusCode, errorContent);
//             throw new HttpRequestException($"Falha ao processar fatura: {response.StatusCode}");
//         }
//
//         var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
//         return ParseGeminiResponse(jsonResponse);
//     }
//
//     private string BuildPrompt(string categoryContext, string fileName)
//     {
//         var currentRef = DateTime.Now.ToString("MM/yyyy"); 
//
//         return = $@"
//                 Aja como um especialista financeiro analisando faturas de cartão de crédito em PDF.
//
//                 Categorias existentes no sistema:
//                 [{{categoryList}}]
//
//                 Objetivo:
//                 Extrair corretamente as transações financeiras.
//
//                 Regras:
//                 1. Extraia:
//                 - TransactionDate
//                 - Description
//                 - Amount
//
//                 2. Ignore pagamentos de fatura, estornos, créditos ou ajustes.
//
//                 3. Categorias:
//                 - Se houver similaridade, use CategoryId
//                 - Caso contrário, sugira NewCategoryName
//
//                 4. Parcelamentos:
//                 - Identifique 'Parcela X de Y'
//
//                 5. Datas:
//                 - Transações parceladas devem pertencer ao mês da fatura
//
//                 Formato:
//                 Retorne APENAS um array JSON válido:
//                 [
//                   {{{{
//                     """"Description"""": """"string"""",
//                     """"Amount"""": number,
//                     """"TransactionDate"""": """"YYYY-MM-DD"""",
//                     """"CategoryId"""": """"uuid?"""",
//                     """"NewCategoryName"""": """"string?""""
//                   }}}}       
//
//         ]";
//     }
//
//     private IReadOnlyList<AiInvoiceExtractionItem> ParseGeminiResponse(string jsonResponse)
//     {
//         try
//         {
//             using var doc = JsonDocument.Parse(jsonResponse);
//             
//             // Navega até o texto da resposta: candidates[0].content.parts[0].text
//             var root = doc.RootElement;
//             if (!root.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
//                 return Array.Empty<AiInvoiceExtractionItem>();
//
//             var content = candidates[0].GetProperty("content");
//             var parts = content.GetProperty("parts");
//             var text = parts[0].GetProperty("text").GetString();
//
//             if (string.IsNullOrEmpty(text)) 
//                 return Array.Empty<AiInvoiceExtractionItem>();
//
//             // Limpeza básica de Markdown caso a IA desobedeça
//             var cleanJson = text
//                 .Replace("```json", "")
//                 .Replace("```", "")
//                 .Trim();
//
//             var options = new JsonSerializerOptions 
//             { 
//                 PropertyNameCaseInsensitive = true,
//                 NumberHandling = JsonNumberHandling.AllowReadingFromString 
//             };
//
//             return JsonSerializer.Deserialize<List<AiInvoiceExtractionItem>>(cleanJson, options) 
//                    ?? new List<AiInvoiceExtractionItem>();
//         }
//         catch (Exception ex)
//         {
//             logger.LogError(ex, "Erro ao fazer parse do JSON retornado pelo Gemini.");
//             return Array.Empty<AiInvoiceExtractionItem>();
//         }
//     }
// }