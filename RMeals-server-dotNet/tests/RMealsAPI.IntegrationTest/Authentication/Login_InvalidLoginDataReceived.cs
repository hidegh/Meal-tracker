using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Xunit;

namespace RMealsAPI.IntegrationTest
{
    [Collection(MainTestColection.Name)]
    public class Login_InvalidLoginDataReceived
    {
        [Fact]
        public void Answer_should_be_HTTP400()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(HttpHostFixture.UrlBase + "/authentication/login"));

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                { "userName", "saul" },
                { "password", "wrongPassword" }
            });

            var result = HttpHostFixture.HttpClient.SendAsync(request).Result;
            
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
    }
}
