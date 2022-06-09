using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Admin.SearchProfileService.Model;
using Admin.SearchProfileService.Repository.Contracts;
using MongoDB.Driver;

namespace Admin.SearchProfileService.Repository.Implementation
{
    public class SearchProfileRepository : ISearchProfileRepository
    {
        private readonly ISearchProfileContext _context;
        public SearchProfileRepository(ISearchProfileContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> GetEngineerProfilesDetailsRepository(string criteria, string criteriaValue, int? perPage, int? page)
        {
            ApiResponse response = new ApiResponse();
            UserProfilesDetails userProfileDetails = null;
            if (string.Compare(criteria.ToUpper(), "NAME") == 0)
            {
                if (perPage.HasValue && perPage.Value > 0 && page.HasValue && page.Value > 0)
                {
                    List<UserProfileForAdminDatabase> userProfileList = await _context.UserProfileForAdminDatabase
                            .Find(f => f.Name.ToUpper().Contains(criteriaValue.ToUpper()))
                            .Skip((page - 1) * perPage)
                            .Limit(perPage)
                            .ToListAsync();

                    double totalDocuments = await _context.UserProfileForAdminDatabase.CountDocumentsAsync(f => f.Name.ToUpper().Contains(criteriaValue.ToUpper()));
                    if (totalDocuments > 0)
                    {
                        var totalPages = Math.Ceiling(totalDocuments / perPage.Value);

                        userProfileDetails = new UserProfilesDetails()
                        {
                            XTotal = (int)totalDocuments,
                            XTotalPages = (int)totalPages,
                            XPerPage = perPage.Value,
                            XPage = page.Value,
                            UserList = userProfileList
                        };
                    }
                }
                else
                {
                    List<UserProfileForAdminDatabase> userProfileList = await _context.UserProfileForAdminDatabase
                            .Find(f => f.Name.ToUpper().Contains(criteriaValue.ToUpper()))
                            .ToListAsync();

                    double totalDocuments = userProfileList.Count;
                    if (totalDocuments > 0)
                    {
                        userProfileDetails = new UserProfilesDetails()
                        {
                            XTotal = (int)totalDocuments,
                            XTotalPages = 1,
                            XPerPage = 0,
                            XPage = 0,
                            UserList = userProfileList
                        };
                    }
                }
            }
            else if (string.Compare(criteria.ToUpper(), "ASSOCIATE ID") == 0)
            {
                if (perPage.HasValue && perPage.Value > 0 && page.HasValue && page.Value > 0)
                {
                    List<UserProfileForAdminDatabase> userProfileList = await _context.UserProfileForAdminDatabase
                                .Find(f => f.AssociateId.Equals(criteriaValue.ToUpper()))
                                .Skip((page - 1) * perPage)
                                .Limit(perPage)
                                .ToListAsync();
                    
                    double totalDocuments = await _context.UserProfileForAdminDatabase.CountDocumentsAsync(f => f.AssociateId.Contains(criteriaValue.ToUpper()));
                    if (totalDocuments > 0)
                    {
                        var totalPages = Math.Ceiling(totalDocuments / perPage.Value);

                        userProfileDetails = new UserProfilesDetails()
                        {
                            XTotal = (int)totalDocuments,
                            XTotalPages = (int)totalPages,
                            XPerPage = perPage.Value,
                            XPage = page.Value,
                            UserList = userProfileList
                        };
                    }
                }
                else
                {
                    List<UserProfileForAdminDatabase> userProfileList = await _context.UserProfileForAdminDatabase
                            .Find(f => f.AssociateId.Equals(criteriaValue.ToUpper()))
                            .ToListAsync();

                    double totalDocuments = userProfileList.Count;
                    if (totalDocuments > 0)
                    {
                        userProfileDetails = new UserProfilesDetails()
                        {
                            XTotal = (int)totalDocuments,
                            XTotalPages = 1,
                            XPerPage = 0,
                            XPage = 0,
                            UserList = userProfileList
                        };
                    }
                }
            }
            else if (string.Compare(criteria.ToUpper(), "SKILL") == 0)
            {
                if (perPage.HasValue && perPage.Value > 0 && page.HasValue && page.Value > 0)
                {
                    List<UserProfileForAdminDatabase> userProfileList = await _context.UserProfileForAdminDatabase
                            .Find(f => f.TechnicalSkillDetails.Any(o => o.SkillName.Equals(criteriaValue.ToUpper()) && o.SkillValue > 10) ||
                                        f.NonTechnicalSkillDetails.Any(o => o.SkillName.Equals(criteriaValue.ToUpper()) && o.SkillValue > 10))
                            .Skip((page - 1) * perPage)
                            .Limit(perPage)
                            .ToListAsync();

                    double totalDocuments = await _context.UserProfileForAdminDatabase.CountDocumentsAsync(f => f.TechnicalSkillDetails.Any(o => o.SkillName.Equals(criteriaValue.ToUpper()) && o.SkillValue > 10) ||
                                        f.NonTechnicalSkillDetails.Any(o => o.SkillName.Equals(criteriaValue.ToUpper()) && o.SkillValue > 10));
                    if (totalDocuments > 0)
                    {
                        var totalPages = Math.Ceiling(totalDocuments / perPage.Value);

                        userProfileDetails = new UserProfilesDetails()
                        {
                            XTotal = (int)totalDocuments,
                            XTotalPages = (int)totalPages,
                            XPerPage = perPage.Value,
                            XPage = page.Value,
                            UserList = userProfileList
                        };
                    }
                }
                else
                {
                    List<UserProfileForAdminDatabase> userProfileList = await _context.UserProfileForAdminDatabase
                            .Find(f => f.TechnicalSkillDetails.Any(o => o.SkillName.Equals(criteriaValue.ToUpper()) && o.SkillValue > 10) ||
                                        f.NonTechnicalSkillDetails.Any(o => o.SkillName.Equals(criteriaValue.ToUpper()) && o.SkillValue > 10))
                            .ToListAsync();

                    double totalDocuments = userProfileList.Count;
                    if (totalDocuments > 0)
                    {
                        userProfileDetails = new UserProfilesDetails()
                        {
                            XTotal = (int)totalDocuments,
                            XTotalPages = 1,
                            XPerPage = 0,
                            XPage = 0,
                            UserList = userProfileList
                        };
                    }
                }
            }

            if (userProfileDetails != null && userProfileDetails.XTotal > 0)
            {
                userProfileDetails.UserList.ToList().ForEach(u =>
                {
                    u.TechnicalSkillDetails = u.TechnicalSkillDetails.OrderByDescending(o => Convert.ToInt32(o.SkillValue)).ToList();
                    u.NonTechnicalSkillDetails = u.NonTechnicalSkillDetails.OrderByDescending(o => Convert.ToInt32(o.SkillValue)).ToList();
                });
                response.Result = userProfileDetails;
            }
            else
            {
                response.Result = null;
                response.Status.Message = "NO RECORD FOUND";
                response.Status.IsValid = false;
                response.Status.Status = "FAIL";
            }

            return response;
        }

        public async Task InsertUserProfileRepository(UserProfile userProfile)
        {
            List<SkillDetails> techSkillDetails = null, nonTechSkillDetails = null;
            ConvertExpertiseToListOfExpertise(ref techSkillDetails, ref nonTechSkillDetails, userProfile);
            

            UserProfileForAdminDatabase userProfileForAdminDatabase = new UserProfileForAdminDatabase
            {
                UserId = userProfile.UserId,
                Name = userProfile.Name,
                AssociateId = userProfile.AssociateId,
                Mobile = userProfile.Mobile,
                Email = userProfile.Email,
                TechnicalSkillDetails = techSkillDetails.ToList(),
                NonTechnicalSkillDetails = nonTechSkillDetails.ToList(),
                CreatedDate = userProfile.CreatedDate,
                UpdatedDate = userProfile.UpdatedDate
            };

            await _context.UserProfileForAdminDatabase.InsertOneAsync(userProfileForAdminDatabase);
        }

        public async Task<bool> UpdateUserProfileRepository(UserProfile userProfile)
        {
            var userItem = await _context
                            .UserProfileForAdminDatabase
                            .Find(
                                filter: f => f.UserId == userProfile.UserId
                            ).FirstOrDefaultAsync();

            List<SkillDetails> techSkillDetails = null, nonTechSkillDetails = null;
            ConvertExpertiseToListOfExpertise(ref techSkillDetails, ref nonTechSkillDetails, userProfile);


            UserProfileForAdminDatabase userProfileForAdminDatabase = new UserProfileForAdminDatabase
            {
                UserId = userItem.UserId,
                Name = userItem.Name,
                AssociateId = userItem.AssociateId,
                Mobile = userItem.Mobile,
                Email = userItem.Email,
                TechnicalSkillDetails = techSkillDetails.ToList(),
                NonTechnicalSkillDetails = nonTechSkillDetails.ToList(),
                CreatedDate = userItem.CreatedDate,
                UpdatedDate = userProfile.UpdatedDate
            };


            ReplaceOneResult updatedResult = await _context.UserProfileForAdminDatabase.ReplaceOneAsync(
                        filter: f => f.UserId == userProfile.UserId,
                        replacement: userProfileForAdminDatabase
                        );

            return updatedResult != null && updatedResult.IsAcknowledged && updatedResult.ModifiedCount > 0;
        }

        private void ConvertExpertiseToListOfExpertise(ref List<SkillDetails> techSkillDetails, ref List<SkillDetails> nonTechSkillDetails, UserProfile userProfile)
        {
            techSkillDetails = new List<SkillDetails>
            {
                new SkillDetails { SkillName = "HTML-CSS-JAVASCRIPT", SkillValue = Convert.ToInt32(userProfile.TechnicalSkillExpertiseLevel.HTMLCSSJavaScriptExpertiseLevel) },
                new SkillDetails { SkillName = "ANGULAR", SkillValue = Convert.ToInt32(userProfile.TechnicalSkillExpertiseLevel.AngularExpertiseLevel) },
                new SkillDetails { SkillName = "REACT", SkillValue = Convert.ToInt32(userProfile.TechnicalSkillExpertiseLevel.ReactExpertiseLevel) },
                new SkillDetails { SkillName = "ASP.NET CORE", SkillValue = Convert.ToInt32(userProfile.TechnicalSkillExpertiseLevel.AspNetCoreExpertiseLevel) },
                new SkillDetails { SkillName = "RESTFUL", SkillValue = Convert.ToInt32(userProfile.TechnicalSkillExpertiseLevel.RestfulExpertiseLevel) },
                new SkillDetails { SkillName = "ENTITY FRAMEWORK", SkillValue = Convert.ToInt32(userProfile.TechnicalSkillExpertiseLevel.EntityFrameworkExpertiseLevel) },
                new SkillDetails { SkillName = "GIT", SkillValue = Convert.ToInt32(userProfile.TechnicalSkillExpertiseLevel.GitExpertiseLevel) },
                new SkillDetails { SkillName = "DOCKER", SkillValue = Convert.ToInt32(userProfile.TechnicalSkillExpertiseLevel.DockerExpertiseLevel) },
                new SkillDetails { SkillName = "JENKINS", SkillValue = Convert.ToInt32(userProfile.TechnicalSkillExpertiseLevel.JenkinsExpertiseLevel) },
                new SkillDetails { SkillName = "AZURE", SkillValue = Convert.ToInt32(userProfile.TechnicalSkillExpertiseLevel.AzureExpertiseLevel) }
            };

            nonTechSkillDetails = new List<SkillDetails>()
            {
                new SkillDetails { SkillName = "SPOKEN", SkillValue = Convert.ToInt32(userProfile.NonTechnicalSkillExpertiseLevel.SpokenExpertiseLevel) },
                new SkillDetails { SkillName = "COMMUNICATION", SkillValue = Convert.ToInt32(userProfile.NonTechnicalSkillExpertiseLevel.CommunicationExpertiseLevel) },
                new SkillDetails { SkillName = "APTITUDE", SkillValue = Convert.ToInt32(userProfile.NonTechnicalSkillExpertiseLevel.AptitudeExpertiseLevel) }
            };
        }
    }
}