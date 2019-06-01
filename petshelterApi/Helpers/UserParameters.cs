using System.Collections.Specialized;

namespace petshelterApi.Helpers
{
    public class UserParameters
    {
        const int maxPageSize = 100;
        public int PageNumber { get; set; } = 0;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
        public string SearchQuery { get; set; }
        public string OrderBy { get; set; } = "email:1";

        public string QueryString
        {
            get
            {
                var queryString = CreateSearchQuery();
                AddParameter(ref queryString, "per_page", PageSize);
                AddParameter(ref queryString, "page", PageNumber);
                AddParameter(ref queryString, "sort", OrderBy);
                AddParameter(ref queryString, "include_totals", "true");
                AddParameter(ref queryString, "search_engine", "v3");
                return queryString.TrimEnd('&');
            }
        }

        private void AddParameter(ref string queryString, string parameterName, object value)
        {
            queryString += $"{parameterName}={value}&";
        }

        // Auth0 management API /users uses Lucene query syntax
        // http://www.lucenetutorial.com/lucene-query-syntax.html
        private string CreateSearchQuery()
        {
            var query = "?";
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                return query;
            }
            query += "q=";

            var queryPairs = new NameValueCollection() { { "username", SearchQuery }, { "email", SearchQuery } };
            for (int i = 0; i < queryPairs.Count; i++)
            {
                query += $"{queryPairs.GetKey(i)}:*{queryPairs.Get(i)}* OR ";
            }
            return query.TrimEnd(" OR ".ToCharArray()) + '&';
        }
    }
}
