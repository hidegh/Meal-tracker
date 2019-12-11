using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using RMealsAPI.Model;
using Xunit;

namespace RMealsAPI.IntegrationTest
{
    [TestCaseOrderer("RMealsAPI.IntegrationTest.PriorityOrderer", "RMealsAPI.IntegrationTest")]
    [Collection(MainTestColection.Name)]
    public class Manager_AlteringRoles: IClassFixture<Manager_AlteringRoles.Setup>
    {
        public class Setup
        {
            public long RegularUserId;
            public long ManagerUserId;
            public long AdminUserId;
            public string AuthorizationToken;

            public Setup()
            {
                RegularUserId = HttpHostFixture.GetUserId("paul");
                ManagerUserId = HttpHostFixture.GetUserId("manager");
                AdminUserId = HttpHostFixture.GetUserId("admin");
                AuthorizationToken = HttpHostFixture.GetTokenFor("manager", "manager123$");
            }
        }

        private readonly Setup fixture;

        public Manager_AlteringRoles(Setup fixture)
        {
            this.fixture = fixture;
        }

        [TestPriority(1)]
        [Fact]
        public void Manager_can_set_manager_role()
        {
            var roles = new string[] { RoleConsts.Manager };
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
        public void Manager_can_remove_manager_role()
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

        [TestPriority(3)]
        [Fact]
        public void Manager_cant_set_admin_role()
        {
            var roles = new string[] { RoleConsts.Admin };
            var rolesJson = JsonConvert.SerializeObject(roles);

            var request = new HttpRequestMessage(HttpMethod.Put, new Uri(HttpHostFixture.UrlBase + $"/users/{fixture.ManagerUserId}/roles"));
            request.Content = new System.Net.Http.StringContent(rolesJson, Encoding.UTF8, "application/json");
            HttpHostFixture.AppendAuthentication(request, fixture.AuthorizationToken);

            var result = HttpHostFixture.HttpClient.SendAsync(request).Result;
            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [TestPriority(4)]
        [Fact]
        public void Manager_cant_remove_admin_role()
        {
            var roles = new string[] { };
            var rolesJson = JsonConvert.SerializeObject(roles);

            var request = new HttpRequestMessage(HttpMethod.Put, new Uri(HttpHostFixture.UrlBase + $"/users/{fixture.AdminUserId}/roles"));
            request.Content = new System.Net.Http.StringContent(rolesJson, Encoding.UTF8, "application/json");
            HttpHostFixture.AppendAuthentication(request, fixture.AuthorizationToken);

            var result = HttpHostFixture.HttpClient.SendAsync(request).Result;
            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

    }
}
