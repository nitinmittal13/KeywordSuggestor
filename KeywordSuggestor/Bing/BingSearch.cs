using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace KeywordSuggestor.Bing
{
    /// <summary>
    /// Register for bing search API at https://datamarket.azure.com/dataset/bing/search
    /// </summary>
    internal class BingSearch
    {
        private readonly string accountKey;

        private const string EndpointFormat = "https://api.datamarket.azure.com/Bing/Search/v1/Composite?Sources=%27Web%27&Query=%27{0}%27";

        public BingSearch()
        {
            this.accountKey = ConfigurationManager.AppSettings["BingApiKey"];
        }

        public string SearchEndpoint
        {
            get
            {
                return EndpointFormat;
            }
        }

        internal NetworkCredential Credential
        {
            get { return new NetworkCredential(this.accountKey, this.accountKey);}
        }

        public SearchResult Search(string terms)
        {

            if (string.IsNullOrWhiteSpace(terms))
            {
                throw  new ArgumentNullException("terms", "search term cannot be null or empty.");
            }

            var request = WebRequest.CreateHttp(string.Format(this.SearchEndpoint, terms));
            request.Method = "GET";
            request.Credentials = this.Credential;
            //request.Proxy = new WebProxy("127.0.0.1", 8888);
            var response = request.GetResponse();
            var webCount = this.GetWebCount(response.GetResponseStream());
            
            return new SearchResult{Total = webCount, SearchTerm = terms};
        }

        private Int64 GetWebCount(Stream strem)
        {
            var xDoc = XDocument.Load(strem);

            if (xDoc != null)
            {
                xDoc.StripNamespace();
                var contentNode = xDoc.Descendants("content").LastOrDefault();
                var webCount = contentNode.Descendants("WebTotal").FirstOrDefault().Value;
                return Convert.ToInt64(webCount);
            }
            return 0;
        }
        
    }
}
