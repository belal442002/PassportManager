
using Azure;
using Document_Intelligence_Task.Domain.Models;
using Microsoft.Extensions.Primitives;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Document_Intelligence_Task.Services
{
    public class DocumentIntelligenceService : IDocumentIntelligenceService
    {
        private readonly HttpClient _httpClient;

        public DocumentIntelligenceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> AnalyzeDocumentAsync(string documentUrl)
        {
            var analyzeUrl = "documentintelligence/documentModels/prebuilt-idDocument:analyze?api-version=2024-11-30";
            var requestBody = new { urlSource = documentUrl };

            var response = await _httpClient.PostAsJsonAsync(analyzeUrl, requestBody);

            response.EnsureSuccessStatusCode();
            
            if(response.Headers.TryGetValues("Operation-Location",out var operationLocationValues))
            {
                return operationLocationValues.FirstOrDefault()!;
            }

            throw new Exception("Operation-Location header not found in the response.");
        }

        public async Task<string> AnalyzeLocalDocumentAsync(Stream fileStream, string fileName)
        {
            try
            {
                var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(fileStream);

                var mimeType = GetMimeType(fileName);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);

                content.Add(fileContent, "file", fileName);

                var analyzeUrl = "/documentintelligence/documentModels/prebuilt-idDocument:analyze?_overload=analyzeDocument&api-version=2024-11-30";

                var response = await _httpClient.PostAsync(analyzeUrl, content);
                response.EnsureSuccessStatusCode();

                if(response.Headers.TryGetValues("Operation-Location", out var operationLocationValues))
                {
                    return operationLocationValues.FirstOrDefault()!;
                }

                throw new Exception("Operation-Location header not found in the response.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GetMimeType(string fileName)
        {
            var MimeTypeMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { ".pdf", "application/pdf" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".png", "image/png" },
                { ".tiff", "image/tiff" },
                { ".bmp", "image/bmp" },
            };

            var extension = Path.GetExtension(fileName);

            if(MimeTypeMappings.TryGetValue(extension, out var mimeType))
            {
                return mimeType;
            }

            throw new NotSupportedException($"File type '{extension}' is not supported.");
        }

        public async Task<string> GetAnalyzeResultAsync(string operationalLocation)
        {
            while (true)
            {
                var response = await _httpClient.GetAsync(operationalLocation);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                if(result.Contains("\"status\":\"succeeded\""))
                {
                    return result;
                }

                await Task.Delay(3000);
                
            }     
        }

        public IDDocument_Passport Parse_IDDocumentResult(string jsonResult)
        {
            var result = JsonSerializer.Deserialize<JsonElement>(jsonResult);

            var content = result.GetProperty("analyzeResult").GetProperty("content").GetString();

            // Split the content into lines
            var lines = content!.Split('\n');
            var linesCount = lines.Length;

            // regex to fetch the dates inside the passport
            string pattern = @"^\d{2} (JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)$";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            List<string> dates = new List<string>();

            for(int i = 0; i < linesCount; i++) 
            {
                var lineParts = lines[i].Split("/"); // Suppose lines[i] = "19 JAN / JAN" or "04 DEC /DÉC 88"
                var part1 = lineParts[0].Trim(); // Then part1 = "19 JAN" or "04 DEC"
                if (regex.IsMatch(part1)) // If it is actually "19 JAN" or "04 DEC"
                {
                    dates.Add(part1);
                    if(lineParts.Length == 2) // if it is actually "19 JAN / JAN" or "04 DEC /DÉC 88"
                    {
                        string year = string.Empty;
                        var part2 = lineParts[1].Trim(); //Then part2 = "JAN" or "DÉC 88"
                        if(part2.Split(" ").Length == 2) // If it is "DÉC 88"
                        {
                            year = part2.Split(" ")[1]; // year = "88";
                            if (Convert.ToInt32(year) >= 50)
                                year = "19" + year; // year = 1988
                            else
                                year = "20" + year; // For example year = "2018"
                            
                        }
                        else // "JAN"
                        {
                            var secondLine = i + 1 < linesCount ? lines[i + 1].Trim() : string.Empty;
                            var thirdLine = i + 2 < linesCount ? lines[(i + 2)].Trim() : string.Empty;

                            if(!string.IsNullOrWhiteSpace(secondLine) 
                                && int.TryParse(secondLine.Split(" ")[0].Trim(), out var secondLineVal))
                            {
                                year = secondLineVal.ToString();
                                if (secondLineVal >= 50)
                                    year = "19" + year; // year = 1988
                                else
                                    year = "20" + year; // year = 2021
                            }
                            else if(!string.IsNullOrWhiteSpace(secondLine)
                                && int.TryParse(thirdLine.Split(" ")[0].Trim(), out var thirdLineVal))
                            {
                                year = thirdLineVal.ToString();
                                if (thirdLineVal >= 50)
                                    year = "19" + year; // year = 1988
                                else
                                    year = "20" + year; // year = 2021
                            }
                        }
                        dates[dates.Count - 1] = dates[dates.Count - 1] + " " + year; // "dd MMM yyyy" 
                    }
                }
            }

            var passport = new IDDocument_Passport
            {
                DocumentId = Guid.NewGuid(),
                DocumentType = 6 >= linesCount ? null : lines[6],
                FirstName = 12 >= linesCount ? null : lines[12],
                LastName = 10 >= linesCount ? null : lines[10],
                Nationality = 14 >= linesCount ? null : lines[14],
                Sex = lines.Contains("f", StringComparer.OrdinalIgnoreCase) ? "F"
                     :lines.Contains("m", StringComparer.OrdinalIgnoreCase) ? "M" : null,
                DateOfBirth = ParseDate(dates[0]),
                DateOfIssue = ParseDate(dates[1]),
                DateOfExpiration = ParseDate(dates[2]),
                PlaceOfBirth = GetDateOfBirth(lines),
                IssuingAuthority = GetDateOfBirth(lines),
                DocumentNumber = GetPassportNumber(lines),
                PersonalNumber = GetPassportNumber(lines),
                CountryRegion = GetCountryCode(lines),
            };

            return passport;
        }

        private DateTime? ParseDate(string dateString)
        {
            if (DateTime.TryParseExact(dateString, "dd MMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }
            return null;
        }

        private string? GetDateOfBirth(string[] lines)
        {
            for(int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Equals("f", StringComparison.OrdinalIgnoreCase)
                 || lines[i].Equals("m", StringComparison.OrdinalIgnoreCase))
                {
                    return i + 1 >= lines.Length ? null : lines[i+1];
                }
            }
            return null;
        }

        private string? GetPassportNumber(string[] lines)
        {
            var max = 0;
            foreach(string line in lines)
            {
                if(int.TryParse(line, out var number))
                {
                    if(number > max)
                        max = number;
                }
            }
            return max == 0 ? null : max.ToString();
        }

        private string? GetCountryCode(string[] lines)
        {
            foreach(string line in lines)
            {
                if (line.Length == 3)
                    return line;
            }
            return null;
        }
    }
}
