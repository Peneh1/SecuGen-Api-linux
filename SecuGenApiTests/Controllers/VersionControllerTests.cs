using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace SecuGenApi.Controllers.Tests
{
    [TestClass()]
    public class VersionControllerTests
    {
        public string baseUrl = "http://localhost:5167/api";

        [TestMethod()]
        public void GetTest()
        {
            string? data = null;

            using (var httpClient = new HttpClient()) {
                httpClient.BaseAddress = new Uri(baseUrl);

                string apiController = "Version";
                //string apiAction = "";

                var url = baseUrl + "/" + apiController;//SecuGenApiTests.Helpers.beSmartUriBuilder(baseUrl, apiController, null);

                var response = httpClient.GetAsync(url).Result;
                if (response != null)
                {
                    var v = response.Content.ToString();
                    if(v != null)
                        data = JsonSerializer.Deserialize<string>(v);
                }
                else
                    Assert.Fail();
            }

        }
    }
}