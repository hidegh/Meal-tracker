using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Xunit;

namespace RMealsAPI.IntegrationTest
{
    [Collection(MainTestColection.Name)]
    public class Registration_ValidDataReceived : IClassFixture<Registration_ValidDataReceived.Setup>
    {
        public class Setup
        {
            public HttpResponseMessage result;

            public Setup()
            {
                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(HttpHostFixture.UrlBase + "/authentication/register"));

                request.Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                    { "userName", "newUser" },
                    { "password", "newUser123$" }
                });

                result = HttpHostFixture.HttpClient.SendAsync(request).Result;
            }
        }

        private readonly Setup fixture;

        public Registration_ValidDataReceived(Setup fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Answer_is_HTTP200()
        {
            
            Assert.Equal(HttpStatusCode.OK, fixture.result.StatusCode);
        }

        [Fact]
        public void Answer_contains_token_and_expiration()
        {
            var contentString = fixture.result.Content.ReadAsStringAsync().Result;

            var definition = new { Token = "", Expiration = (DateTime?)DateTime.Now };
            var responseJson = JsonConvert.DeserializeAnonymousType(contentString, definition);

            Assert.False(string.IsNullOrWhiteSpace(responseJson.Token), "Token was not provided by endrpoint");
            Assert.True(responseJson.Expiration.HasValue, "Expiration date was not provided by endpoint");
        }

    }
}
