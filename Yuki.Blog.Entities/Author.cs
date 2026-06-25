namespace Yuki.Blog.Entities
{
    public record Author(int Id, string Name, string Surname)
    {
        public Author() : this(default, string.Empty, string.Empty) // Only here to enable XML serialization
        {
        }
    }
}
