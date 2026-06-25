using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Yuki.Blog.Entities;
using Yuki.Blog.Services;

namespace Yuki.Blog.Tests.Integration
{
    public class BlogDataFixture : IAsyncLifetime
    {
        public WebApplicationFactory<Program>? Factory { get; private set; }

        public ValueTask InitializeAsync()
        {
            // Wrap the WebApplicationFactory, since we need it as part of the fixture
            Factory = new WebApplicationFactory<Program>();
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IBlogService>() ?? throw new Exception($"Initialization failed for {nameof(BlogDataFixture)}: {nameof(IBlogService)} not registered.");

            // Create two blog posts as test data, bypassing the HTTP layer but still keeping validation logic
            var post = new BlogPost(default, default, "Blog Title", "Blog Description", "Blog Content");
            post.Author = new(default, "New", "Author");

            var result = service.SaveBlogPost(post);
            if (!result.IsValid)
                throw new Exception($"Initialization failed for {nameof(BlogDataFixture)}.");

            result = service.SaveBlogPost(post);
            if (!result.IsValid)
                throw new Exception($"Initialization failed for {nameof(BlogDataFixture)}.");

            return ValueTask.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            if (Factory is not null)
                await Factory.DisposeAsync();
        }
    }

    public class GetBlogData : BlogDataFixture { }
    public class DeleteBlogData : BlogDataFixture { }
}
