using System.Text.Json.Serialization;
using Yuki.Blog.Entities;

namespace Yuki.Blog.API
{
    [JsonSerializable(typeof(List<string>))]
    [JsonSerializable(typeof(Author))]
    [JsonSerializable(typeof(BlogPost[]))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    public partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}
