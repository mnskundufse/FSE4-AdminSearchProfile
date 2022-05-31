using Admin.SearchProfileService.Model;
using System.Threading.Tasks;

namespace Admin.SearchProfileService.Business.Contracts
{
    public interface ISearchProfileBusiness
    {
        Task<ApiResponse> GetEngineerProfilesDetailsBusiness(string criteria, string criteriaValue, int? perPage, int? page);
    }
}
