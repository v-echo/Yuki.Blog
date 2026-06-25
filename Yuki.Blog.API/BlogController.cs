using Asp.Versioning;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Yuki.Blog.Entities;
using Yuki.Blog.Services;

namespace Yuki.Blog.API
{
    [ApiVersion(1)]
    [ApiController]
    [Route("api/blog")]
    public class BlogController(IBlogService service, ILogger<BlogController> logger) : ControllerBase
    {
        [HttpGet]
        [Route("{id:int}")]
        [MapToApiVersion(1)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<BlogPost> Get(int id, string? include)
        {
            try
            {
                var blog = service.GetBlogPost(id, include);

                if (blog.Errors.Count > 0)
                {
                    logger.LogError("Request failed: {url}. Errors: {err}", HttpContext.Request.GetDisplayUrl(), blog.Errors);
                    return BadRequest(blog.Errors);
                }

                if (blog.Content is null)
                    return NotFound();

                return Ok(blog.Content);
            }
            catch (Exception e) // The try-catch could be done in an exception filter instead
            {
                logger.LogError(e, "Request failed: {url}", HttpContext.Request.GetDisplayUrl()); // Depending on the hosting platform and their logging capabilities or integrations, this could be unnecessary
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("")]
        [MapToApiVersion(1)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<BlogPost> Post(BlogPost post)
        {
            try
            {
                var created = service.SaveBlogPost(post);

                if (!created.IsValid)
                    return BadRequest(created.Errors);

                return Created($"/api/blog/{created.Content!.Id}", created.Content);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Request failed: {url}", HttpContext.Request.GetDisplayUrl());
                return StatusCode(500);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [MapToApiVersion(1)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<BlogPost> Delete(int id)
        {
            try
            {
                var deleted = service.DeleteBlogPost(id);

                if (deleted.IsValid)
                    return Ok(deleted.Content);

                return NotFound();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Request failed: {url}", HttpContext.Request.GetDisplayUrl());
                return StatusCode(500);
            }
        }
    }
}
