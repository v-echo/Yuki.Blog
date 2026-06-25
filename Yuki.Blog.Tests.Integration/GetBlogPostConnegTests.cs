using System.Net;
using System.Net.Http.Json;
using System.Xml.Serialization;
using Yuki.Blog.Entities;

namespace Yuki.Blog.Tests.Integration
{
    public class GetBlogPostConnegTests(GetBlogData fixture) : IClassFixture<GetBlogData>
    {
        public static readonly TheoryData<string, int> BlogAPIVersions = new()
        {
            { "/api/blog", 1 } // Only v1 API supports content negotiation
        };

        [Theory]
        [MemberData(nameof(BlogAPIVersions))]
        public async Task Get_Existing_BlogPost_AsJson(string url, int version)
        {
            using var client = GetClient(version);
            var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/0?include=author");
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new("application/json"));

            var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var post = await response.Content.ReadFromJsonAsync<BlogPost>(TestContext.Current.CancellationToken);

            Assert.NotNull(post);
            Assert.NotNull(post.Author);
        }

        [Theory]
        [MemberData(nameof(BlogAPIVersions))]
        public async Task Get_Existing_BlogPost_AsXML(string url, int version)
        {
            using var client = GetClient(version);
            var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/0?include=author");
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new("application/xml"));

            var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var serializer = new XmlSerializer(typeof(BlogPost));
            var post = serializer.Deserialize(response.Content.ReadAsStream(TestContext.Current.CancellationToken)) as BlogPost;

            Assert.NotNull(post);
            Assert.NotNull(post.Author);
        }

        private HttpClient GetClient(int version)
        {
            var client = fixture.Factory!.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-Version", version.ToString()); // This is fine because we dispose of the client after each test
            return client;
        }
    }
}
