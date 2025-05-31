using Document_Intelligence_Task.Data;
using Document_Intelligence_Task.Domain.Models;
using Document_Intelligence_Task.Interfaces;
using Document_Intelligence_Task.Services;
using System.Text.Json;

namespace Document_Intelligence_Task.Repositories
{
    public class PassportRepository : GenericRepository<IDDocument_Passport> , IPassportRepository
    {
        private readonly DocumentIntelligenceDB _dbContext;

        public PassportRepository(DocumentIntelligenceDB dbContext) : base(dbContext) 
        {
            _dbContext = dbContext;
        }
        

    }
}
