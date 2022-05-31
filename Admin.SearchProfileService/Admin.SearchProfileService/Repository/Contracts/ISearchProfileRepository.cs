using Admin.SearchProfileService.Model;
using System.Threading.Tasks;

namespace Admin.SearchProfileService.Repository.Contracts
{
    public interface ISearchProfileRepository
    {
        Task<ApiResponse> GetEngineerProfilesDetailsRepository(string criteria, string criteriaValue, int? perPage, int? page);
        Task InsertUserProfileRepository(UserProfile userProfile);
        Task<bool> UpdateUserProfileRepository(UserProfile userProfile);
    }
}
