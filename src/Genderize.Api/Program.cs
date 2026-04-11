using Genderize.Api.Configurations;
using Genderize.Api.Helpers;
using Genderize.Api.Interfaces;
using Genderize.Api.Middleware;
using Genderize.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<GenderizeOptions>(
    builder.Configuration.GetSection(GenderizeOptions.SectionName));

builder.Services.AddHttpClient(HttpClientNames.GenderizeApi, (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var genderizeSection = configuration.GetSection(GenderizeOptions.SectionName);
    var baseUrl = genderizeSection["BaseUrl"];

    client.BaseAddress = new Uri(baseUrl!);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddScoped<IGenderizeService, GenderizeService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
    status = "success",
    message = "API is running"
}));

app.Run();
