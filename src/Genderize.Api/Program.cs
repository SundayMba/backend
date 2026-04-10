using Genderize.Api.Configurations;
using Genderize.Api.Interfaces;
using Genderize.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add framework services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure options
builder.Services.Configure<GenderizeOptions>(
    builder.Configuration.GetSection(GenderizeOptions.SectionName));

// Register HttpClient for Genderize service
builder.Services.AddHttpClient<IGenderizeService, GenderizeService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var genderizeSection = configuration.GetSection(GenderizeOptions.SectionName);
    var baseUrl = genderizeSection["BaseUrl"];

    client.BaseAddress = new Uri(baseUrl!);
    client.Timeout = TimeSpan.FromSeconds(10);
});

// Configure CORS to allow all origins for HNG grading
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

// Enable Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
    status = "success",
    message = "API is running"
}));

app.Run();