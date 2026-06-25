using System.Net;
using System.Net.Http.Json;
using Yuki.Blog.Entities;

namespace Yuki.Blog.Tests.Integration
{
    public class GetBlogPostAPITests(GetBlogData fixture) : IClassFixture<GetBlogData>
    {
        public static readonly TheoryData<string, int> BlogAPIVersions = new()
        {
            { "/api/blog", 1 },
            { "/api/blog", 2 }
        };

        [Theory]
        [MemberData(nameof(BlogAPIVersions))]
        public async Task Get_NonExisting_BlogPost(string url, int version)
        {
            using var client = GetClient(version);
            var response = await client.GetAsync($"{url}/{int.MaxValue}", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(BlogAPIVersions))]
        public async Task Get_Existing_BlogPost_Invalid_Id(string url, int version)
        {
            using var client = GetClient(version);
            var response = await client.GetAsync($"{url}/{-1}", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(BlogAPIVersions))]
        public async Task Get_Existing_BlogPost_Without_Author(string url, int version)
        {
            using var client = GetClient(version);
            var response = await client.GetAsync($"{url}/0", TestContext.Current.CancellationToken);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var post = await response.Content.ReadFromJsonAsync<BlogPost>(TestContext.Current.CancellationToken);

            Assert.NotNull(post);
            Assert.Null(post.Author);
        }

        [Theory]
        [MemberData(nameof(BlogAPIVersions))]
        public async Task Get_Existing_BlogPost_With_Author(string url, int version)
        {
            using var client = GetClient(version);
            var response = await client.GetAsync($"{url}/0?include={nameof(BlogPost.Author)}", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var post = await response.Content.ReadFromJsonAsync<BlogPost>(TestContext.Current.CancellationToken);

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
