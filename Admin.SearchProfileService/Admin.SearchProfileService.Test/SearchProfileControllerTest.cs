using System.Threading.Tasks;
using Admin.SearchProfileService.Business.Contracts;
using Admin.SearchProfileService.Controllers;
using Admin.SearchProfileService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Admin.SearchProfileService.Test
{
    public class SearchProfileControllerTest
    {

        Mock<ISearchProfileBusiness> _mockBusiness = new Mock<ISearchProfileBusiness>();
        readonly Mock<ILogger<SearchProfileController>> _mockLogger = new Mock<ILogger<SearchProfileController>>();
        [Fact]
        public async Task GetEngineerProfilesDetails_ValidRequest()
        {
            UserProfile request = new UserProfile
            {
                AssociateId = "CTS03",
                Name = "manas",
                Mobile = "3231453213",
                Email = "manas@gmail.com"
            };

            ApiResponse response = new ApiResponse()
            {
                Result = new UserProfilesDetails
                {
                    XTotal = 30,
                    XPage = 1,
                    XPerPage = 10
                },
                Status = new StatusResponse
                {
                    IsValid = true,
                    Status = "SUCCESS",
                    Message = string.Empty
                }
            };

            SearchProfileController _testObject = new SearchProfileController(_mockLogger.Object, _mockBusiness.Object);
            _mockBusiness.Setup(x => x.GetEngineerProfilesDetailsBusiness(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(response));

            var result = (ObjectResult)await _testObject.GetEngineerProfilesDetails("NAME", "Manas", 10, 3);
            UserProfilesDetails profile = (UserProfilesDetails)((ApiResponse)result.Value).Result;
            Assert.Equal(10, profile.XPerPage);
        }
    }
}
