using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Yuki.Blog.Entities;

namespace Yuki.Blog.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterBlogServices(this IServiceCollection services)
        {
            services.AddSingleton<IBlogService, BlogService>(); // Singleton because in this implementation, it holds state; normally it would be scoped.
            services.AddScoped<IValidator<BlogPost>, BlogPostValidator>();
            services.AddScoped<IValidator<Author>, AuthorValidator>();
            return services;
        }
    }
}
