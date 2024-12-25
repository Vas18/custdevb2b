using Azure.Core;
using custdev.domain.configuration;
using custdev.domain.db;
using Microsoft.EntityFrameworkCore;

namespace custdev.db.cosmos.context
{
    public class AppMsgContext : DbContext
    {
        private readonly CosmosDbConfiguration _cosmosConfig;

        public DbSet<DbMessage> DbMessages { get; set; }

        public AppMsgContext(DbContextOptions<AppMsgContext> options) : base(options)
        {
        }

        public AppMsgContext(CosmosDbConfiguration cosmosConfig)
        {
            _cosmosConfig = cosmosConfig;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmos(
            _cosmosConfig.Endpoint,
            _cosmosConfig.AccountPrimaryKey,
            _cosmosConfig.MsgDbName);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // configuring CvRequests
            modelBuilder.Entity<DbMessage>()
                    .ToContainer("dbMessages") // ToContainer
                    .HasPartitionKey(e => e.RequestId); // Partition Key
        }

    }
}
