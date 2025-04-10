using FlashCard.Api.Data;
using FlashCard.Api.Models;
using FlashCard.Api.Services;
using FlashCard.Api.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "FlashCard API", 
        Version = "v1",
        Description = "API for generating flashcards using AI"
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure DbContext
builder.Services.AddDbContext<FlashCardDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure OpenRouter
builder.Services.Configure<OpenRouterOptions>(
    builder.Configuration.GetSection(OpenRouterOptions.SectionName));
builder.Services.AddSingleton<IValidateOptions<OpenRouterOptions>, OpenRouterOptionsValidator>();

// Configure Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddAuthorization();

// Configure Services
builder.Services.AddHttpClient<IGenerationService, GenerationService>();
builder.Services.AddScoped<IGenerationService, GenerationService>();

var app = builder.Build();

// Validate OpenRouter configuration
var openRouterOptions = app.Services.GetRequiredService<IOptions<OpenRouterOptions>>().Value;
var openRouterValidator = app.Services.GetRequiredService<IValidateOptions<OpenRouterOptions>>();
var validationResult = openRouterValidator.Validate(null, openRouterOptions);

if (validationResult.Failed)
{
    throw new InvalidOperationException(
        $"Invalid OpenRouter configuration: {string.Join(", ", validationResult.Failures)}");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FlashCardDbContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating the database.");
    }
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Add endpoints
app.MapPost("/generations", async (
    GenerationRequestDto request,
    IGenerationService generationService,
    ClaimsPrincipal user,
    CancellationToken cancellationToken) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
        throw new UnauthorizedAccessException("User not authenticated"));
        
    var result = await generationService.GenerateFlashcardsAsync(request, userId, cancellationToken);
    return Results.Created($"/generations/{result.Id}", result);
})
.RequireAuthorization()
.WithName("GenerateFlashcards")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
