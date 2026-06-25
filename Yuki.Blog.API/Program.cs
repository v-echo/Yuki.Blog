using Asp.Versioning;
using Scalar.AspNetCore;
using Yuki.Blog.API;
using Yuki.Blog.Services;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddControllers(options => // Doesn't work with AoT
{
    options.RespectBrowserAcceptHeader = true; // Enable content negotiation, but only for controllers
}).AddXmlSerializerFormatters();

builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("X-API-Version"), new QueryStringApiVersionReader("api-version"));
    options.DefaultApiVersion = new ApiVersion(1);
    options.AssumeDefaultVersionWhenUnspecified = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
}).AddMvc();
/*.AddOpenApi()*/; // Version mismatch between Asp.Versioning and Microsoft.OpenAPI packages requires upgrade to .NET 10
builder.Services.AddOpenApi("v1").AddOpenApi("v2");
builder.Services.RegisterBlogServices();

var app = builder.Build();
app.MapOpenApi()/*.WithDocumentPerVersion()*/;
app.NewVersionedApi("Blog").MapBlogApi();
app.MapScalarApiReference(options => // This is the visual representation of the API
{
    var descriptions = app.DescribeApiVersions();

    for (var i = 0; i < descriptions.Count; i++)
    {
        var description = descriptions[i];
        var isDefault = i == descriptions.Count - 1;
        options.AddDocument(description.GroupName, description.GroupName, isDefault: isDefault);
    }
});
app.MapControllers();
app.Run();

public partial class Program { } // Workaround for WebApplicationFactory entry point