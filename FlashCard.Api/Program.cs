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
using FlashCard.Api.Services.OpenRouter;

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
                "http://localhost:5007"
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

// Configure HttpClient for OpenRouter
builder.Services.AddHttpClient<IOpenRouterService, OpenRouterService>();

// Configure Authentication
var jwtLogger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"];
    var jwtIssuer = builder.Configuration["Jwt:Issuer"];
    var jwtAudience = builder.Configuration["Jwt:Audience"];
    
    jwtLogger.LogInformation("JWT Configuration:");
    jwtLogger.LogInformation("Issuer: {Issuer}", jwtIssuer);
    jwtLogger.LogInformation("Audience: {Audience}", jwtAudience);
    jwtLogger.LogInformation("Key length: {KeyLength}", jwtKey?.Length ?? 0);
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey ?? throw new InvalidOperationException("JWT Key is missing")))
    };
    
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            jwtLogger.LogError(context.Exception, "Authentication failed");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            jwtLogger.LogInformation("Token validated successfully");
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            jwtLogger.LogInformation("Token received: {Token}", context.Token);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Configure Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOpenRouterService, OpenRouterService>();
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


app.MapPost("/api/generations", async (
    GenerationRequestDto request,
    IGenerationService generationService,
    ClaimsPrincipal user,
    ILogger<Program> logger,
    HttpContext httpContext,
    CancellationToken cancellationToken) =>
{
    logger.LogInformation("Received request to /api/generations");
    
    // Log headers
    var authHeader = httpContext.Request.Headers["Authorization"].ToString();
    logger.LogInformation("Authorization header: {AuthHeader}", authHeader);
    
    // Log user claims
    if (user?.Identity?.IsAuthenticated == true)
    {
        logger.LogInformation("User is authenticated");
        foreach (var claim in user.Claims)
        {
            logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
        }
    }
    else
    {
        logger.LogWarning("User is not authenticated");
    }

    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
        throw new UnauthorizedAccessException("User not authenticated"));
        
    logger.LogInformation("Processing request for user ID: {UserId}", userId);
    
    var result = await generationService.GenerateFlashcardsAsync(request, userId, cancellationToken);
    return Results.Created($"/api/generations/{result.Id}", result);
})
.RequireAuthorization()
.WithName("GenerateFlashcards");

// Add flashcards batch endpoint
app.MapPost("/api/flashcards/batch", async (BatchSaveRequest request, FlashCardDbContext dbContext,
    ClaimsPrincipal user, CancellationToken cancellationToken) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
        throw new UnauthorizedAccessException("User not authenticated"));

    var flashcards = request.Flashcards.Select(f => new Flashcard
    {
        UserId = userId,
        Front = f.Front,
        Back = f.Back,
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

app.Run();
