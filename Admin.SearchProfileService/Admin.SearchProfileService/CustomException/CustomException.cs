using System;
namespace Admin.SearchProfileService.CustomException
{
    [Serializable]
    public class InvalidSearchCriteriaException : Exception
    {
        public InvalidSearchCriteriaException() { }

        public InvalidSearchCriteriaException(string criteria)
            : base(String.Format("Invalid Search Criteria {0}. Search can be possible with criteria as 'NAME', 'ASSOCIATE ID' or 'SKILL'.", criteria))
        {

        }
    }
}
