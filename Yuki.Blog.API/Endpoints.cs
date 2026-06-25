using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Yuki.Blog.Entities;
using Yuki.Blog.Services;

namespace Yuki.Blog.API
{
    public static class Endpoints
    {
        public static IVersionedEndpointRouteBuilder MapBlogApi(this IVersionedEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("api/blog").HasApiVersion(2);

            group.MapGet("{id}", async Task<Results<Ok<BlogPost>, BadRequest<List<string>>, NotFound, InternalServerError>> (int id, string? include, HttpContext context, IBlogService service, ILogger<IBlogService> logger) => 
            {
                try
                {
                    var blog = service.GetBlogPost(id, include);

                    if (blog.Errors.Count > 0)
                    {
                        logger.LogError("Request failed: {url}. Errors: {err}", context.Request.GetDisplayUrl(), blog.Errors);
                        return TypedResults.BadRequest(blog.Errors);
                    }

                    if (blog.Content is null)
                        return TypedResults.NotFound();

                    return TypedResults.Ok(blog.Content);
                }
                catch (Exception e) // The try-catch could be done in a middleware globally instead
                {
                    logger.LogError(e, "Request failed: {url}", context.Request.GetDisplayUrl()); // Depending on the hosting platform and their logging capabilities or integrations, this could be unnecessary
                    return TypedResults.InternalServerError(); // What to include here in the response is dependent on the intended audience and security restrictions of the platform
                }
            });

            group.MapPost("", async Task<Results<Created<BlogPost>, BadRequest<List<string>>, InternalServerError>> (BlogPost post, HttpContext context, IBlogService service, ILogger<IBlogService> logger) => 
            {
                try
                {
                    var created = service.SaveBlogPost(post);

                    if (!created.IsValid)
                        return TypedResults.BadRequest(created.Errors);

                    return TypedResults.Created($"/api/blog/{created.Content!.Id}", created.Content);
                }
                catch (Exception e) 
                {
                    logger.LogError(e, "Request failed: {url}", context.Request.GetDisplayUrl());
                    return TypedResults.InternalServerError();
                }
            });

            group.MapDelete("{id}", async Task<Results<Ok<BlogPost>, NotFound, InternalServerError>> (int id, HttpContext context, IBlogService service, ILogger<IBlogService> logger) =>
            {
                try
                {
                    var deleted = service.DeleteBlogPost(id);

                    if (deleted.IsValid)
                        return TypedResults.Ok(deleted.Content);

                    return TypedResults.NotFound();
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Request failed: {url}", context.Request.GetDisplayUrl());
                    return TypedResults.InternalServerError();
                }
            });

            return builder;
        }
    }
}
