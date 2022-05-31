using Admin.SearchProfileService.Config;

namespace Admin.SearchProfileService.Models
{
    public class ServerConfig
    {
        public MongoDbConfig MongoDB { get; set; } = new MongoDbConfig();
    }
}