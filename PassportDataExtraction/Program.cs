using Document_Intelligence_Task.Data;
using Document_Intelligence_Task.Services;
using Document_Intelligence_Task.UOW;
using Microsoft.EntityFrameworkCore;

namespace Document_Intelligence_Task
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<DocumentIntelligenceDB>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DocumentIntelligenceDB"));
            });

            // Read the endpoint and API key from configuration
            var azure_DocumentIntelligence_Configuration
                = builder.Configuration.GetSection("AzureDocumentIntelligence");
            var endpoint = azure_DocumentIntelligence_Configuration["Endpoint"];
            var apiKey = azure_DocumentIntelligence_Configuration["ApiKey"];

            // Register the DocumentIntelligenceService as a typed HTTP client to leverage the benefits of IHttpClientFactory
            builder.Services.AddHttpClient<IDocumentIntelligenceService, DocumentIntelligenceService>(client =>
            {
                client.BaseAddress = new Uri(endpoint!);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
            });

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IFileService, FileService>();
            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();

            app.Run();
        }
    }
}
