using Yuki.Blog.Entities;

namespace Yuki.Blog.Services
{
    public interface IBlogService
    {
        Result<BlogPost> GetBlogPost(int id, string? include = null);
        Result<BlogPost> SaveBlogPost(BlogPost post);
        Result<BlogPost> DeleteBlogPost(int id);
    }
}