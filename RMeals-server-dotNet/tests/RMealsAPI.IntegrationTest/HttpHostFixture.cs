using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using RMealsAPI.Features.Users;

namespace RMealsAPI.IntegrationTest
{
    /// <summary>
    /// Provides a shared context for all the test classes...
    /// https://github.com/aspnet/AspNetCore/blob/master/src/Hosting/Abstractions/src/WebHostDefaults.cs
    /// </summary>
    public class HttpHostFixture : IDisposable
    {
        public static string UrlBase = "http://localhost:41000";
        public static HttpClient HttpClient = HttpClientFactory.Create();

        public static string GetTokenFor(string userName, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(HttpHostFixture.UrlBase + "/authentication/login"));

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                    { "userName", userName },
                    { "password", password }
                });

            var result = HttpClient.SendAsync(request).Result;
            var contentString = result.Content.ReadAsStringAsync().Result;

            var definition = new { Token = "", Expiration = (DateTime?)DateTime.Now };
            var responseJson = JsonConvert.DeserializeAnonymousType(contentString, definition);

            return responseJson.Token;
        }

        public static void AppendAuthentication(HttpRequestMessage message, string token)
        {
            message.Headers.Add(HttpRequestHeader.Authorization.ToString(), "Bearer " + token);
        }

        public static long GetUserId(string userName)
        {
            var token = HttpHostFixture.GetTokenFor("admin", "admin123$");
            var requestUri = QueryHelpers.AddQueryString(new Uri(HttpHostFixture.UrlBase + "/users").ToString(), new Dictionary<string, string>() { });
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            HttpHostFixture.AppendAuthentication(request, token);

            var result = HttpClient.SendAsync(request).Result;
            var json = result.Content.ReadAsStringAsync().Result;

            var userList = JsonConvert.DeserializeObject<List<UserDetailsDto>>(json);

            return userList.First(u => u.Name == userName).Id;
        }



        private CancellationTokenSource cancellationToken;
        private Task task;

        public HttpHostFixture()
        {
            var args = $"--environment Test --urls {HttpHostFixture.UrlBase}";

            // NOTE: would block rest
            // RMealsAPI.Program.Main(args.Split(" "));

            cancellationToken = new CancellationTokenSource();
            task = Task.Factory.StartNew(
                () => {
                    RMealsAPI.Program.Main(args.Split(" "));
                },
                cancellationToken.Token
            );

            // NOTE: due DB re-creation (not in-memory) we need to wait for the endpoint to come alive...
            Thread.Sleep(15000);
        }

        public void Dispose()
        {
            cancellationToken.Cancel();
        }
    }

}
