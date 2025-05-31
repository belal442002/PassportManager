using Document_Intelligence_Task.Domain.Models;

namespace Document_Intelligence_Task.Services
{
    public interface IDocumentIntelligenceService
    {
        Task<string> AnalyzeDocumentAsync(string documentUrl);
        Task<string> GetAnalyzeResultAsync(string operationalLocation);
        Task<string> AnalyzeLocalDocumentAsync(Stream fileStream, string fileName);
        IDDocument_Passport Parse_IDDocumentResult(string jsonResult);
        
    }
}
