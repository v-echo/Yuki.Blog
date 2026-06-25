using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using Yuki.Blog.Entities;

namespace Yuki.Blog.Services
{
    public class BlogService(IServiceScopeFactory factory) : IBlogService
    {
        ConcurrentDictionary<int, BlogPost> BlogPosts { get; } = new();
        ConcurrentDictionary<int, Author> Authors { get; } = new();

        public Result<BlogPost> GetBlogPost(int id, string? include = null)
        {
            if (id < 0)
                return new Result<BlogPost>().AddError("Id cannot be negative");

            string[] includes = include?.Split(';') ?? [];

            if (BlogPosts.TryGetValue(id, out var post))
            {
                if (includes.Contains(nameof(Author), StringComparer.InvariantCultureIgnoreCase))
                {
                    if (Authors.TryGetValue(post.AuthorId, out var author))
                        post.Author = author;
                    else return new Result<BlogPost>().AddError("Author not found");
                }
                else post.Author = null;
            }

#pragma warning disable CS8604 // Intentional. Null is a valid result here.
            return new Result<BlogPost>(post);
#pragma warning restore CS8604
        }

        public Result<BlogPost> SaveBlogPost(BlogPost post)
        {
            // Get scoped validator instance
            using var scope = factory.CreateScope();
            var validator = scope.ServiceProvider.GetService<IValidator<BlogPost>>() ?? throw new Exception($"Missing {nameof(BlogPost)} validator!");

            // Validate blog post instance
            var result = validator.Validate(post);

            if (!result.IsValid)
            {
                var response = new Result<BlogPost>();
                result.Errors.ForEach(error => response.AddError(error.ErrorMessage));
                return response;
            }

            // Check for an Author by id
            if (Authors.TryGetValue(post.AuthorId, out var author))
            {
                post.Author = author;
            }
            else
            {
                if (post.Author is null) // Alternatively, we could have a "Default Author" for posts without one
                    return new Result<BlogPost>().AddError("No author specified for this blog post");

                // Try to find the author by name and surname. The find value operation is very inefficient for a dictionary, but this would be replaced by a proper DB in a real scenario.
                var existing = Authors.FirstOrDefault(kv => string.Equals(kv.Value.Name, post.Author.Name) && string.Equals(kv.Value.Surname, post.Author.Surname));
                if (existing.Value is not null)
                {
                    post = post with { AuthorId = existing.Value.Id };
                    post.Author = existing.Value;
                }
                else
                {
                    // If the author doesn't exist yet, create a new entry
                    var id = GetNextAuthorId();
                    post = post with { AuthorId = id };
                    post.Author = post.Author with { Id = id };
                    Authors.TryAdd(id, post.Author);
                }
            }

            // Generate an id for the new blog post
            int i = 0;
            while (BlogPosts.ContainsKey(i))
                i++;
            post = post with { Id = i };

            // Save the blog post
            i = 0;
            while (!BlogPosts.TryAdd(post.Id, post) && i++ < 3) // Race condition check
                post = post with { Id = post.Id + 1 };

            return new Result<BlogPost>(post);

            int GetNextAuthorId()
            {
                int i = 0;
                while (Authors.ContainsKey(i))
                    i++;
                return i;
            }
        }

        public Result<BlogPost> DeleteBlogPost(int id)
        {
            // Check if the blog post exists. The author doesn't need to be deleted, as it can exist independently.
            if (BlogPosts.TryRemove(id, out var post))
                return new Result<BlogPost>(post);

            return new Result<BlogPost>();
        }
    }
}
