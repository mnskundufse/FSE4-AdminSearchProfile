using Admin.SearchProfileService.Model;
using MongoDB.Driver;

namespace Admin.SearchProfileService.Repository.Contracts
{
    public interface ISearchProfileContext
    {
        IMongoCollection<UserProfileForAdminDatabase> UserProfileForAdminDatabase { get; }
    }
}
