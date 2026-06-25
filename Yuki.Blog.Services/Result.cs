namespace Yuki.Blog.Services
{
    public record Result<T> where T : class
    {
        public T? Content { get; init; }
        public List<string> Errors { get; } = [];
        public bool IsValid => Content is not null && Errors.Count == 0;

        public Result()
        {
        }

        public Result(T content)
        {
            Content = content;
        }

        public Result<T> AddError(string error)
        {
            Errors.Add(error);
            return this;
        }
    }
}
