using Xunit;
using Yuki.Blog.Entities;
using Yuki.Blog.Services;

namespace Yuki.Blog.Tests
{
    public class ValidatorTests
    {
#pragma warning disable CS8625 // Intentional
        [Fact]
        public void IsValid_Author()
        {
            var author = new Author(1, "Test", "Author");
            var validator = new AuthorValidator();

            Assert.True(validator.Validate(author).IsValid);
        }

        [Fact]
        public void IsInvalid_Author_Name()
        {
            var validator = new AuthorValidator();
            var author = new Author(1, "", "Author");
            Assert.False(validator.Validate(author).IsValid);

            author = author with { Name = null };
            Assert.False(validator.Validate(author).IsValid);
        }

        [Fact]
        public void IsInvalid_Author_Surname()
        {
            var validator = new AuthorValidator();
            var author = new Author(1, "Test", "");
            Assert.False(validator.Validate(author).IsValid);

            author = author with { Surname = null };
            Assert.False(validator.Validate(author).IsValid);
        }

        [Fact]
        public void IsValid_BlogPost()
        {
            var validator = new BlogPostValidator();
            var post = new BlogPost(1, 1, "Title", "A blog", "Text content");
            Assert.True(validator.Validate(post).IsValid);

            post.Author = new(1, "Blog", "Author");
            Assert.True(validator.Validate(post).IsValid);
        }

        [Fact]
        public void IsInvalid_BlogPost_Title()
        {
            var validator = new BlogPostValidator();
            var post = new BlogPost(1, 1, "", "A blog", "Text content");
            Assert.False(validator.Validate(post).IsValid);

            post = post with { Title = null };
            Assert.False(validator.Validate(post).IsValid);

            post = post with { Title = "    " };
            Assert.False(validator.Validate(post).IsValid);

            post = post with { Title = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum." };
            Assert.False(validator.Validate(post).IsValid);
        }

        [Fact]
        public void IsInvalid_BlogPost_Content()
        {
            var validator = new BlogPostValidator();
            var post = new BlogPost(1, 1, "", "A blog", "Text content");
            Assert.False(validator.Validate(post).IsValid);

            post = post with { Title = null };
            Assert.False(validator.Validate(post).IsValid);

            post = post with { Title = "    " };
            Assert.False(validator.Validate(post).IsValid);
        }

        [Fact]
        public void IsInvalid_BlogPost_Author()
        {
            var validator = new BlogPostValidator();
            var post = new BlogPost(1, 1, "Title", "A blog", "Text content");
            post.Author = new(1, "", "Author");
            Assert.False(validator.Validate(post).IsValid);
        }
#pragma warning restore CS8625
    }
}
