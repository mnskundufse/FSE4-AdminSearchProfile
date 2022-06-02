﻿using System;
using Xunit;
using Moq;
using Admin.SearchProfileService.Business.Contracts;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Confluent.Kafka;
using Admin.SearchProfileService.Controllers;
using Admin.SearchProfileService.Model;
using Admin.SearchProfileService.Kafka;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Admin.SearchProfileService.Business.Implementation;
using Admin.SearchProfileService.Repository.Contracts;
using Admin.SearchProfileService;
using Admin.SearchProfileService.CustomException;

namespace Admin.SearchProfileService.Test
{
    public class SearchProfileControllerTest
    {
        readonly Mock<ISearchProfileRepository> _mockRepo = new Mock<ISearchProfileRepository>();
        [Fact]
        public async Task GetEngineerProfilesDetails_ValidRequest()
        {
            ApiResponse response = new ApiResponse()
            {
                Result = new UserProfilesDetails
                {
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

            SearchProfileBusiness _testObject = new SearchProfileBusiness(_mockRepo.Object);
            _mockRepo.Setup(x => x.GetEngineerProfilesDetailsRepository(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(response));

            var result = await _testObject.GetEngineerProfilesDetailsBusiness("NAME", "Manas", 10, 3);
            UserProfilesDetails profile = (UserProfilesDetails)((ApiResponse)result).Result;
            Assert.Equal(10, profile.XPerPage);
        }
        /// <summary>
        /// test for invalid name criteria
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetEngineerProfilesDetails_InvalidNameRequest()
        {
            SearchProfileBusiness _testObject = new SearchProfileBusiness(_mockRepo.Object);
            var result = await _testObject.GetEngineerProfilesDetailsBusiness("NAME", "Mana*", 10, 3);
            Assert.Equal("Criteria value for Name is not valid. Initial characters of name can be provided.", ((ApiResponse)result).Status.Message);
        }
        /// <summary>
        /// test for invalid associate id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetEngineerProfilesDetails_InvalidAssociateIdRequest()
        {
            SearchProfileBusiness _testObject = new SearchProfileBusiness(_mockRepo.Object);
            var result = await _testObject.GetEngineerProfilesDetailsBusiness("ASSOCIATE ID", "Mana", 10, 3);
            Assert.Equal("Criteria value for Associate Id is not valid. AssociateId can't be NULL, and must start with 'CTS'.", ((ApiResponse)result).Status.Message);
        }
        /// <summary>
        /// test for invalid skill request
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetEngineerProfilesDetails_InvalidSkillIdRequest()
        {
            SearchProfileBusiness _testObject = new SearchProfileBusiness(_mockRepo.Object);
            var result = await _testObject.GetEngineerProfilesDetailsBusiness("SKILL", "Mana", 10, 3);
            Assert.Equal("Criteria value for SKILL is not valid.", ((ApiResponse)result).Status.Message);
        }
        /// <summary>
        /// test for invaid search result
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetEngineerProfilesDetails_InvalidSearchRequest()
        {
            SearchProfileBusiness _testObject = new SearchProfileBusiness(_mockRepo.Object);
            var result =await Assert.ThrowsAsync<InvalidSearchCriteriaException>(()=> _testObject.GetEngineerProfilesDetailsBusiness("SKILLTEST", "Mana", 10, 3));
            Assert.Equal("Invalid Search Criteria SKILLTEST. Search can be possible with criteria as 'NAME', 'ASSOCIATE ID' or 'SKILL'.", result.Message);
        }
    }
}
