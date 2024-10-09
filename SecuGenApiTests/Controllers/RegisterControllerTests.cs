using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace SecuGenApi.Controllers.Tests
{
    [TestClass()]
    public class RegisterControllerTests
    {
        public string baseUrl = "blahblah";

        [TestMethod()]
        public void RegisterTest()
        {
            string? data = null;

            using (var httpClient = new HttpClient()) {
                httpClient.BaseAddress = new Uri(baseUrl);

                string apiController = "Version";
                string apiAction = "Get";

                var url = SecuGenApiTests.Helpers.beSmartUriBuilder(baseUrl, apiController, apiAction);

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