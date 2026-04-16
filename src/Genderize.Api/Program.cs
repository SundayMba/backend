using Genderize.Api.Configurations;
using Genderize.Api.Data;
using Genderize.Api.Helpers;
using Genderize.Api.Interfaces;
using Genderize.Api.Middleware;
using Genderize.Api.Models.Responses;
using Genderize.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            return new BadRequestObjectResult(ApiErrorResponse.Create("Invalid request payload"));
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<GenderizeOptions>(
    builder.Configuration.GetSection(GenderizeOptions.SectionName));
builder.Services.Configure<AgifyOptions>(
    builder.Configuration.GetSection(AgifyOptions.SectionName));
builder.Services.Configure<NationalizeOptions>(
    builder.Configuration.GetSection(NationalizeOptions.SectionName));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHttpClient(HttpClientNames.GenderizeApi, (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var genderizeSection = configuration.GetSection(GenderizeOptions.SectionName);
    var baseUrl = genderizeSection["BaseUrl"];

    client.BaseAddress = new Uri(baseUrl!);
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddHttpClient(HttpClientNames.AgifyApi, (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var section = configuration.GetSection(AgifyOptions.SectionName);
    var baseUrl = section["BaseUrl"];

    client.BaseAddress = new Uri(baseUrl!);
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddHttpClient(HttpClientNames.NationalizeApi, (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var section = configuration.GetSection(NationalizeOptions.SectionName);
    var baseUrl = section["BaseUrl"];

    client.BaseAddress = new Uri(baseUrl!);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddScoped<IGenderizeService, GenderizeService>();
builder.Services.AddScoped<IProfileClassificationService, ProfileClassificationService>();
builder.Services.AddScoped<IProfileService, ProfileService>();

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

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.Use(async (context, next) =>
{
    context.Response.Headers["Access-Control-Allow-Origin"] = "*";
    context.Response.Headers["Access-Control-Allow-Headers"] = "*";
    context.Response.Headers["Access-Control-Allow-Methods"] = "*";

    await next();
});

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
    status = "success",
    message = "API is running"
}));

app.Run();
