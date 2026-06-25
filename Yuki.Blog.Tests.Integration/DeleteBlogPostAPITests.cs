using System.Net;
using System.Net.Http.Json;
using Yuki.Blog.Entities;

namespace Yuki.Blog.Tests.Integration
{
    public class DeleteBlogPostAPITests(DeleteBlogData fixture) : IClassFixture<DeleteBlogData>
    {
        public static readonly TheoryData<string, int> BlogAPIVersions = new()
        {
            { "/api/blog", 1 },
            { "/api/blog", 2 }
        };

        [Theory]
        [MemberData(nameof(BlogAPIVersions))]
        public async Task Delete_Existing_BlogPost(string url, int version)
        {
            using var client = GetClient(version);
            var response = await client.DeleteAsync($"{url}/{version - 1}", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var post = await response.Content.ReadFromJsonAsync<BlogPost>(TestContext.Current.CancellationToken);

            Assert.NotNull(post);
        }

        [Theory]
        [MemberData(nameof(BlogAPIVersions))]
        public async Task Delete_NonExisting_BlogPost(string url, int version)
        {
            using var client = GetClient(version);
            var response = await client.DeleteAsync($"{url}/{int.MaxValue}", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private HttpClient GetClient(int version)
        {
            var client = fixture.Factory!.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-Version", version.ToString()); // This is fine because we dispose of the client after each test
            return client;
        }
    }
}
