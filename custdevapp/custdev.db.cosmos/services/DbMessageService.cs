using custdev.db.cosmos.context;
using custdev.domain.configuration;
using custdev.domain.db;
using custdev.domain.interfaces.db;
using Microsoft.Extensions.Options;

namespace custdev.db.cosmos.services
{
    public class DbMessageService : IDbMessageService
    {
        private readonly AppMsgContext _dbContext;

        public DbMessageService(IOptions<CosmosDbConfiguration> cosmosConfig)
        {
            _dbContext = new AppMsgContext(cosmosConfig.Value);
        }
        public DbMessageService(CosmosDbConfiguration cosmosConfig)
        {
            _dbContext = new AppMsgContext(cosmosConfig);
        }


        public async Task<bool> AddDbMessage(DbMessage model)
        {

            _dbContext.DbMessages.Add(model);
            var success = await _dbContext.SaveChangesAsync();
            return success > 0;
        }
    }
}
