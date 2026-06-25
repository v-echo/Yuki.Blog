using FluentValidation;
using Yuki.Blog.Entities;

namespace Yuki.Blog.Services
{
    public class AuthorValidator : AbstractValidator<Author>
    {
        public AuthorValidator()
        {
            RuleFor(f => f.Name).NotEmpty();
            RuleFor(f => f.Surname).NotEmpty();
        }
    }
}
