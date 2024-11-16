using SmsRelay.Services;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:4000");

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSingleton<MessageRateLimiter>();

var app = builder.Build();

// Enable routing for controllers and setup endpoints
app.UseRouting();
app.MapControllers(); 

app.Run();
