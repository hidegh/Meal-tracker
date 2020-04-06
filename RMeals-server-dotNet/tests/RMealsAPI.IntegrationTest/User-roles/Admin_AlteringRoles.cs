using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using RMealsAPI.Model;
using Xunit;

namespace RMealsAPI.IntegrationTest
{
    [TestCaseOrderer("RMealsAPI.IntegrationTest.PriorityOrderer", "RMealsAPI.IntegrationTest")]
    [Collection(MainTestColection.Name)]
    public class Admin_AlteringRoles: IClassFixture<Admin_AlteringRoles.Setup>
    {
        public class Setup
        {
            public long RegularUserId;
            public string AuthorizationToken;

            public Setup()
            {
                RegularUserId = HttpHostFixture.GetUserId("paul");
                AuthorizationToken = HttpHostFixture.GetTokenFor("admin", "admin123$");
            }
        }

        private readonly Setup fixture;

        public Admin_AlteringRoles(Setup fixture)
        {
            this.fixture = fixture;
        }

        [TestPriority(1)]
        [Fact]
        public void Administrator_can_set_any_role()
        {
            var roles = new string[] { RoleConsts.Admin, RoleConsts.Manager };
            var rolesJson = JsonConvert.SerializeObject(roles);

            var request = new HttpRequestMessage(HttpMethod.Put, new Uri(HttpHostFixture.UrlBase + $"/users/{fixture.RegularUserId}/roles"));
            request.Content = new System.Net.Http.StringContent(rolesJson, Encoding.UTF8, "application/json");
            HttpHostFixture.AppendAuthentication(request, fixture.AuthorizationToken);

            var result = HttpHostFixture.HttpClient.SendAsync(request).Result;
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            // NOTE: we'd also fetch and check it roles were persisted
        }

        [TestPriority(2)]
        [Fact]
        public void Administrator_can_remove_any_role()
        {
            var roles = new string[] { };
            var rolesJson = JsonConvert.SerializeObject(roles);

            var request = new HttpRequestMessage(HttpMethod.Put, new Uri(HttpHostFixture.UrlBase + $"/users/{fixture.RegularUserId}/roles"));
            request.Content = new System.Net.Http.StringContent(rolesJson, Encoding.UTF8, "application/json");
            HttpHostFixture.AppendAuthentication(request, fixture.AuthorizationToken);

            var result = HttpHostFixture.HttpClient.SendAsync(request).Result;
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            // NOTE: we'd also fetch and check it roles were persisted
        }

    }
}
