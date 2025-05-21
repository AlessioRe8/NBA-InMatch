using progetto_cloud.Api.Controllers;
using progetto_cloud.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddEndpointsApiExplorer();

//Opzioni aggiunte per mostrare i nomi delle Division e delle Conference nello Swagger (invece dell'enum)
builder.Services.AddSwaggerGen(options =>
{
    options.UseInlineDefinitionsForEnums();
    options.SchemaGeneratorOptions = new Swashbuckle.AspNetCore.SwaggerGen.SchemaGeneratorOptions
    {
        SchemaIdSelector = type => type.FullName
    };
    options.MapType<Conference>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Enum = new List<Microsoft.OpenApi.Any.IOpenApiAny>
        {
            new Microsoft.OpenApi.Any.OpenApiString("East"),
            new Microsoft.OpenApi.Any.OpenApiString("West")
        }
    });

    options.MapType<Division>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Enum = new List<Microsoft.OpenApi.Any.IOpenApiAny>
        {
            new Microsoft.OpenApi.Any.OpenApiString("Atlantic"),
            new Microsoft.OpenApi.Any.OpenApiString("Central"),
            new Microsoft.OpenApi.Any.OpenApiString("Southeast"),
            new Microsoft.OpenApi.Any.OpenApiString("Northwest"),
            new Microsoft.OpenApi.Any.OpenApiString("Pacific"),
            new Microsoft.OpenApi.Any.OpenApiString("Southwest")
        }
    });
});


builder.Services.AddHttpClient("balldontlie", client =>
{
    client.BaseAddress = new Uri("https://api.balldontlie.io/v1/");
    client.DefaultRequestHeaders.Add("Authorization", "103af612-8d78-484e-938a-bc3ec83b6329");
});

builder.Services.AddControllers();



var app = builder.Build();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();