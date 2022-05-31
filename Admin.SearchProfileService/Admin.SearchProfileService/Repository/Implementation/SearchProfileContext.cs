using Admin.SearchProfileService.Config;
using Admin.SearchProfileService.Model;
using Admin.SearchProfileService.Repository.Contracts;
using MongoDB.Driver;

namespace Admin.SearchProfileService.Repository.Implementation
{
    public class SearchProfileContext : ISearchProfileContext
    {
        private readonly IMongoDatabase _db;

        public SearchProfileContext(MongoDbConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            _db = client.GetDatabase(config.Database);
        }
        public IMongoCollection<UserProfileForAdminDatabase> UserProfileForAdminDatabase => _db.GetCollection<UserProfileForAdminDatabase>("UserProfileForAdmin");
    }
}
