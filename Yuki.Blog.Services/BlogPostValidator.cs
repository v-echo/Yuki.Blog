using FluentValidation;
using Yuki.Blog.Entities;

namespace Yuki.Blog.Services
{
    public class BlogPostValidator : AbstractValidator<BlogPost>
    {
        public BlogPostValidator()
        {
            RuleFor(f => f.Title).NotEmpty().MinimumLength(1).MaximumLength(300);
            RuleFor(f => f.Content).NotEmpty();
#pragma warning disable CS8620 // If the child property is null, then the child validator will not be executed.
            RuleFor(f => f.Author).SetValidator(new AuthorValidator());
#pragma warning restore CS8620
        }
    }
}
