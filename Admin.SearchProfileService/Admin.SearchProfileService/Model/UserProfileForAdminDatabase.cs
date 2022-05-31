using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Admin.SearchProfileService.Model
{
    public class UserProfileForAdminDatabase
    {
        [BsonId]
        public long UserId { get; set; }

        public string Name { get; set; }
        public string AssociateId { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }

        public List<SkillDetails> TechnicalSkillDetails { get; set; }
        public List<SkillDetails> NonTechnicalSkillDetails { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class SkillDetails
    {
        public string SkillName { get; set; }
        public int SkillValue { get; set; }
    }
}
