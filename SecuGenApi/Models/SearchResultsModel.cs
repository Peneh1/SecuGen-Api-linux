namespace SecuGenApi.Models
{
    public class SearchResultsModel
    {
        public bool SearchIdentity { get; set; }
        public bool SearchMatches { get; set; }

        public Int32 SearchResultIdentity { get; set; }

        private List<string> strSearchResultCandidates = new List<string>();

        public SearchResultsModel()
        {
            Init();
        }

        public void Init()
        {
            SearchIdentity = false;
            SearchMatches = false;
            SearchResultIdentity = -1;
            strSearchResultCandidates.Clear();
        }

        public void AddResultCandidate(string strCandidate)
        {
            strSearchResultCandidates.Add(strCandidate);
        }
    }
}
