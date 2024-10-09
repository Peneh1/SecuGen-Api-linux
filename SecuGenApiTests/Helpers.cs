using System.Collections;
using System.Web;

namespace SecuGenApiTests {
    internal static class Helpers
    {
        public static string beSmartUriBuilder(string BaseUrl, string Controller, string Action, IDictionary? Keys = null) {
            string bse = string.Format("{0}/{1}/{2}", BaseUrl, Controller, Action);
            var builder = new UriBuilder(bse);
            var query = HttpUtility.ParseQueryString(builder.Query);

            try {
                if (Keys != null) {
                    foreach (DictionaryEntry kvp in Keys) {
                        query.Add(kvp.Key.ToString(), kvp.Value!.ToString());
                    }
                }
            }
            catch (Exception) {
                throw;
            }
            builder.Query = query.ToString();

            return builder.ToString();
        }

    }
}
