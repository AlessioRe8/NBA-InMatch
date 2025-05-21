using progetto_cloud.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configura HttpClient nominato per balldontlie
builder.Services.AddHttpClient("balldontlie", client =>
{
    client.BaseAddress = new Uri("https://api.balldontlie.io/v1/");
    client.DefaultRequestHeaders.Add("Authorization", "103af612-8d78-484e-938a-bc3ec83b6329");
});

// Registra il servizio per l'accesso ai dati balldontlie
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
