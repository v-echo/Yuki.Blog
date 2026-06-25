using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Yuki.Blog.API;
using Yuki.Blog.Entities;

namespace Yuki.Blog.Tests.Integration
{
    public class CreateBlogPostAPITests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        public static readonly TheoryData<string, int> BlogAPIVersions = new ()
        {
            { "/api/blog", 1 },
            { "/api/blog", 2 }
        };

        [Theory]
        [MemberData(nameof(BlogAPIVersions))]
        public async Task Create_Invalid_BlogPost_Missing_Author(string url, int version)
        {
            var post = new BlogPost(default, int.MaxValue, "Blog Title", "Blog Description", "Blog Content");
            var content = GetBlogPostContent(post, version);

            using var client = factory.CreateClient();
            var response = await client.PostAsync(url, content, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(BlogAPIVersions))]
        public async Task Create_Invalid_BlogPost_Missing_Title(string url, int version)
        {
            var post = new BlogPost(default, int.MaxValue, string.Empty, "Blog Description", "Blog Content");
            post.Author = new(default, "New", "Author");
            var content = GetBlogPostContent(post, version);

            using var client = factory.CreateClient();
            var response = await client.PostAsync(url, content, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(BlogAPIVersions))]
        public async Task Create_Invalid_BlogPost_Missing_Content(string url, int version)
        {
            var post = new BlogPost(default, int.MaxValue, "Blog Title", "Blog Description", string.Empty);
            post.Author = new(default, "New", "Author");
            var content = GetBlogPostContent(post, version);

            using var client = factory.CreateClient();
            var response = await client.PostAsync(url, content, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(BlogAPIVersions))]
        public async Task Create_Valid_BlogPost_New_Author(string url, int version)
        {
            var post = new BlogPost(default, default, "Blog Title", "Blog Description", "Blog Content");
            post.Author = new(default, "New", "Author");
            var content = GetBlogPostContent(post, version);

            using var client = factory.CreateClient();
            var response = await client.PostAsync(url, content, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(BlogAPIVersions))]
        public async Task Create_Valid_BlogPost_Existing_Author_By_Id(string url, int version)
        {
            var post = new BlogPost(default, 0, "Blog Title", "Blog Description", "Blog Content");
            var content = GetBlogPostContent(post, version);
            await Create_Valid_BlogPost_New_Author(url, version); // Since Author is not a primary resource, we need this to create an author first

            using var client = factory.CreateClient();
            var response = await client.PostAsync(url, content, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(BlogAPIVersions))]
        public async Task Create_Valid_BlogPost_Existing_Author_By_Name(string url, int version)
        {
            var post = new BlogPost(default, int.MaxValue, "Blog Title", "Blog Description", "Blog Content");
            post.Author = new(default, "New", "Author");
            var content = GetBlogPostContent(post, version);
            await Create_Valid_BlogPost_New_Author(url, version); // Since Author is not a primary resource, we need this to create an author first

            using var client = factory.CreateClient();
            var response = await client.PostAsync(url, content, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<BlogPost>(TestContext.Current.CancellationToken);
            Assert.Equal(post.Author.Name, result?.Author?.Name);
            Assert.Equal(post.Author.Surname, result?.Author?.Surname);
        }

        public static StringContent GetBlogPostContent(BlogPost post, int version)
        {
            var json = JsonSerializer.Serialize(post, typeof(BlogPost), AppJsonSerializerContext.Default);
            var content = new StringContent(json, new MediaTypeHeaderValue("application/json"));
            content.Headers.Add("X-API-Version", version.ToString());
            return content;
        }
    }
}
