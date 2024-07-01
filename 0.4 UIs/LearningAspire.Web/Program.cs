using LearningAspire.Web;
using LearningAspire.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddRedisOutputCache(Constants.RedisCache);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

//builder.Services.AddHttpClient<WeatherApiClient>(client =>
//    {
//        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
//        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
//        client.BaseAddress = new($"https+http://{Constants.ApiService}");
//    });

var apiServiceUri = new Uri($"https+http://{Constants.ApiService}");
builder.Services.AddHttpClient<WeatherApiClient>(client => client.BaseAddress = apiServiceUri);

// Add a health-check for the weather API backend
builder.Services.AddHealthChecks()
	.AddUrlGroup(new Uri(apiServiceUri, "/health"), name: Constants.ApiService);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
	app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();