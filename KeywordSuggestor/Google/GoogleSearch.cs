using System;
using System.Configuration;

namespace KeywordSuggestor.Google
{
    internal class GoogleSearch
    {
        private readonly string accountKey;

        private const string EndpointFormat = "https://www.googleapis.com/customsearch/v1?key={0}&q={1}&alt=atom";

        public GoogleSearch()
        {
            this.accountKey = ConfigurationManager.AppSettings["GoogleApiKey"];
        }

        public string SearchEndpoint
        {
            get
            {
                return EndpointFormat;
            }
        }

        public SearchResult Search(string terms)
        {
            throw new NotImplementedException("Google Search is not implemented");
        }
    }
}
