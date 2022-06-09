using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Admin.SearchProfileService.Business.Contracts;
using Admin.SearchProfileService.CustomException;
using Admin.SearchProfileService.Model;
using Admin.SearchProfileService.Repository.Contracts;

namespace Admin.SearchProfileService.Business.Implementation
{
    public class SearchProfileBusiness : ISearchProfileBusiness
    {
        public readonly ISearchProfileRepository _repo;
        public SearchProfileBusiness(ISearchProfileRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse> GetEngineerProfilesDetailsBusiness(string criteria, string criteriaValue, int? perPage, int? page)
        {
            ApiResponse response = new ApiResponse();
            if (ValidateRequest(criteria, criteriaValue, ref response))
            {
                response = await _repo.GetEngineerProfilesDetailsRepository(criteria, criteriaValue, perPage, page);
            }
            return response;
        }

        private bool ValidateRequest(string criteria, string criteriaValue, ref ApiResponse response)
        {
            bool isValidated = true;
            if (string.Compare(criteria.ToUpper(), "NAME") == 0)
            {
                if (string.IsNullOrEmpty(criteriaValue) || !Regex.IsMatch(criteriaValue, @"^[a-zA-Z0-9 ]*$"))
                {
                    response.Status.Message = "Criteria value for Name is not valid. Initial characters of name can be provided.";
                    isValidated = false;
                }
            }
            else if (string.Compare(criteria.ToUpper(), "ASSOCIATE ID") == 0)
            {
                if (string.IsNullOrEmpty(criteriaValue) || !criteriaValue.ToUpper().StartsWith("CTS"))
                {
                    response.Status.Message = "Criteria value for Associate Id is not valid. AssociateId can't be NULL, and must start with 'CTS'.";
                    isValidated = false;
                }
            }
            else if (string.Compare(criteria.ToUpper(), "SKILL") == 0)
            {
                List<string> validSkilllist = new List<string>
                {
                    "HTML-CSS-JAVASCRIPT",
                    "ANGULAR",
                    "REACT",
                    "ASP.NET CORE",
                    "RESTFUL",
                    "ENTITY FRAMEWORK",
                    "GIT",
                    "DOCKER",
                    "JENKINS",
                    "AZURE",
                    "SPOKEN",
                    "COMMUNICATION",
                    "APTITUDE"
                };
                if (!validSkilllist.Contains(criteriaValue.ToUpper()))
                {
                    response.Status.Message = "Criteria value for SKILL is not valid.";
                    isValidated = false;
                }
            }
            else
            {
                throw new InvalidSearchCriteriaException(criteria);
            }
            return isValidated;
        }
    }
}
