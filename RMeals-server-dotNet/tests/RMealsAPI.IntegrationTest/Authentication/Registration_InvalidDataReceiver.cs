using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Xunit;

namespace RMealsAPI.IntegrationTest
{
    [Collection(MainTestColection.Name)]
    public class Registration_InvalidLoginDataReceived
    {
        [Fact]
        public void Registering_existing_user_returns_HTTP400()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(HttpHostFixture.UrlBase + "/authentication/register"));

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                { "userName", "saul" },
                { "password", "anyPassword" }
            });

            var result = HttpHostFixture.HttpClient.SendAsync(request).Result;
            
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public void Registering_with_same_username_and_password_returns_HTTP400()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(HttpHostFixture.UrlBase + "/authentication/register"));

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                { "userName", "newUser" },
                { "password", "newUser" }
            });

            var result = HttpHostFixture.HttpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

    }
}
