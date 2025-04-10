using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using FlashCard.App;
using FlashCard.App.Services;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

// Auth services
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

// Configure HttpClient with auth handler
builder.Services.AddScoped<HttpAuthorizationMessageHandler>();
builder.Services.AddHttpClient("API", client => 
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5001");
}).AddHttpMessageHandler<HttpAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

// Other services
builder.Services.AddScoped<IGenerationService, GenerationService>();
builder.Services.AddScoped<IFlashcardService, FlashcardService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();