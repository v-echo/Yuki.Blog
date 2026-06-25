namespace Yuki.Blog.Entities
{
    public record BlogPost (int Id, int AuthorId, string Title, string Description, string Content)
    {
        public Author? Author { get; set; }

        public BlogPost() : this (default, default, string.Empty, string.Empty, string.Empty) // Only here to enable XML serialization
        {
        }
    }
}
