using System;
using System.Net;
using System.Net.Http;
using Xunit;

namespace RMealsAPI.IntegrationTest
{
    [Collection(MainTestColection.Name)]
    public class Administrator_MealsAccess
    {
        [Fact]
        public void Administrator_can_access_meals_of_every_user()
        {
            var userId = HttpHostFixture.GetUserId("saul");

            var token = HttpHostFixture.GetTokenFor("admin", "admin123$");
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(HttpHostFixture.UrlBase + $"/users/{userId}/meals"));
            HttpHostFixture.AppendAuthentication(request, token);

            var result = HttpHostFixture.HttpClient.SendAsync(request).Result;
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

    }
}
