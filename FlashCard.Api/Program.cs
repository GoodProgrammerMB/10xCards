using FlashCard.Api.Data;
using FlashCard.Api.Models;
using FlashCard.Api.Services;
using FlashCard.Api.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
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

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp",
        builder => builder
            .WithOrigins(
                "https://localhost:7174",
                "http://localhost:5007",
                "http://localhost:5170"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Configure DbContext
builder.Services.AddDbContext<FlashCardDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure OpenRouter
builder.Services.Configure<OpenRouterOptions>(
    builder.Configuration.GetSection(OpenRouterOptions.SectionName));
builder.Services.AddSingleton<IValidateOptions<OpenRouterOptions>, OpenRouterOptionsValidator>();

// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();

// Configure Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient<IOpenApiService, OpenApiService>(client =>
{
    var timeout = builder.Configuration.GetValue<int>("OpenRouter:TimeoutSeconds", 120);
    client.Timeout = TimeSpan.FromSeconds(timeout);
});
builder.Services.AddScoped<IOpenApiService, OpenApiService>();
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
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
app.UseCors("AllowBlazorApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FlashCardDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Add endpoints
app.MapPost("/api/generations", async (
    GenerationRequestDto request,
    IOpenApiService openRouterService,
    FlashCardDbContext dbContext,
    ClaimsPrincipal user,
    CancellationToken cancellationToken) =>
{
    string ComputeHash(string input)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
        throw new UnauthorizedAccessException("User not authenticated"));

    // Przygotuj wiadomość dla modelu AI
    var prompt = $"Generate flashcards from the following text: {request.SourceText}";
    
    // Wywołaj OpenRouter API
    var responseContent = await openRouterService.GetChatResponseAsync(
        userMessage: prompt,
        systemMessage: "You are a helpful AI assistant that creates educational flashcards. Each flashcard should have a front (question/term) and back (answer/definition). Generate the flashcards in JSON format as an array of objects with 'front' and 'back' properties.",
        modelName: request.Model,
        cancellationToken: cancellationToken
    );

    // Zapisz generację w bazie danych
    var generation = new Generation
    {
        UserId = userId,
        Name = "Flashcards Generation",
        Model = request.Model ?? openRouterService.DefaultModelName,
        SourceTextHash = ComputeHash(request.SourceText),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    dbContext.Generations.Add(generation);
    await dbContext.SaveChangesAsync(cancellationToken);

    List<GenerationFlashcardDto> flashcards;
    try
    {
        // Dodatkowa sanityzacja na wypadek, gdyby OpenApiService nie naprawił wszystkiego
        //string sanitizedResponse = responseContent.Trim();
        
        //// Sprawdź, czy JSON jest poprawny
        //if (!sanitizedResponse.StartsWith("[") || !sanitizedResponse.EndsWith("]"))
        //{
        //    // Znajdź początek tablicy
        //    var startIndex = sanitizedResponse.IndexOf('[');
        //    if (startIndex >= 0)
        //    {
        //        sanitizedResponse = sanitizedResponse.Substring(startIndex);
                
        //        // Sprawdź, czy jest koniec tablicy
        //        var endIndex = sanitizedResponse.LastIndexOf(']');
        //        if (endIndex > 0)
        //        {
        //            sanitizedResponse = sanitizedResponse.Substring(0, endIndex + 1);
        //        }
        //        else
        //        {
        //            // Usuń końcowy przecinek, jeśli istnieje
        //            sanitizedResponse = sanitizedResponse.TrimEnd();
        //            if (sanitizedResponse.EndsWith(","))
        //            {
        //                sanitizedResponse = sanitizedResponse.Substring(0, sanitizedResponse.Length - 1);
        //            }
        //            // Dodaj końcowy nawias
        //            sanitizedResponse += "]";
        //        }
        //    }
        //}
        
        flashcards = JsonSerializer.Deserialize<List<GenerationFlashcardDto>>(responseContent,
            new JsonSerializerOptions { 
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            }) 
            ?? throw new Exception("Failed to parse flashcards from API response");
    }
    catch (JsonException ex)
    {
        throw new Exception($"Invalid JSON format in API response: {ex.Message}. Response content: {responseContent.Substring(0, Math.Min(200, responseContent.Length))}...");
    }

    generation.GeneratedCount = flashcards.Count;
    await dbContext.SaveChangesAsync(cancellationToken);

    return Results.Created($"/api/generations/{generation.Id}", new GenerationResponseDto
    {
        Id = generation.Id,
        UserId = userId,
        Model = generation.Model,
        GeneratedCount = generation.GeneratedCount,
        Flashcards = flashcards,
        CreatedAt = generation.CreatedAt
    });
})
.RequireAuthorization()
.WithName("GenerateFlashcards");

// Add flashcards batch endpoint
app.MapPost("/api/flashcards/batch", async (
    BatchSaveRequest request,
    FlashCardDbContext dbContext,
    ClaimsPrincipal user,
    CancellationToken cancellationToken) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
        throw new UnauthorizedAccessException("User not authenticated"));

    var flashcards = request.Flashcards.Select(f => new Flashcard
    {
        UserId = userId,
        Front = f.Front,
        Back = f.Back,
        Source = "generated",
        GenerationId = f.GenerationId ?? throw new ArgumentNullException(nameof(f.GenerationId)),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    }).ToList();

    await dbContext.Flashcards.AddRangeAsync(flashcards, cancellationToken);
    await dbContext.SaveChangesAsync(cancellationToken);

    var response = new BatchSaveResponse
    {
        Data = flashcards.Select(f => new SavedFlashcard
        {
            Id = f.Id,
            Front = f.Front,
            Back = f.Back,
            GenerationId = f.GenerationId,
            CreatedAt = f.CreatedAt,
            UpdatedAt = f.UpdatedAt
        }).ToList(),
        Summary = new BatchSaveSummary
        {
            TotalCreated = flashcards.Count,
            TotalFailed = 0
        }
    };

    return Results.Created("/api/flashcards/batch", response);
})
.RequireAuthorization()
.WithName("SaveFlashcardsBatch");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FlashCardDbContext>(); 
    dbContext.Database.Migrate();
}

app.Run();
