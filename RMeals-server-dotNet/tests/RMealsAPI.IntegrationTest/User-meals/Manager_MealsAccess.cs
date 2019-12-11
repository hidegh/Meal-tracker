using System;
using System.Net;
using System.Net.Http;
using Xunit;

namespace RMealsAPI.IntegrationTest
{
    [Collection(MainTestColection.Name)]
    public class Manager_MealsAccess
    {
        [Fact]
        public void Manager_is_forbidden_to_access_meals_of_others()
        {
            var userId = HttpHostFixture.GetUserId("saul");

            var token = HttpHostFixture.GetTokenFor("manager", "manager123$");
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(HttpHostFixture.UrlBase + $"/users/{userId}/meals"));
            HttpHostFixture.AppendAuthentication(request, token);

            var result = HttpHostFixture.HttpClient.SendAsync(request).Result;
            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

    }
}
