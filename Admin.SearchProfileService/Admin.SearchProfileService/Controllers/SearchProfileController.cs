using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Admin.SearchProfileService.Business.Contracts;
using Admin.SearchProfileService.Kafka;
using Admin.SearchProfileService.Model;

namespace Admin.SearchProfileService.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("skill-tracker/api/v{version:apiVersion}/admin")]
    [Produces("application/json")]
    public class SearchProfileController: ControllerBase
    {
        private readonly ILogger<SearchProfileController> _logger;
        private readonly ISearchProfileBusiness _showProfileBC;

        public SearchProfileController(ILogger<SearchProfileController> logger, ISearchProfileBusiness showProfileBC)
        {
            _showProfileBC = showProfileBC;
            _logger = logger;
        }

        [MapToApiVersion("1.0")]
        [Authorize]
        [HttpGet("{criteria}/{criteriaValue}/{perPage?}/{page?}")]
        public async Task<IActionResult> GetEngineerProfilesDetails(string criteria, string criteriaValue, int? perPage, int? page)
        {
            ApiResponse response = await _showProfileBC.GetEngineerProfilesDetailsBusiness(criteria, criteriaValue, perPage, page);
            _logger.LogInformation("{date} : GetEngineerProfilesDetails of the SearchProfileController executed.", DateTime.UtcNow);
            return StatusCode(200, response);
        }
    }
}