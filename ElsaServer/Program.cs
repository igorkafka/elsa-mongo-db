using Elsa.Extensions;
using Elsa.Features.Services;
using Elsa.MongoDb.Extensions;
using Elsa.MongoDb.Modules.Management;
using Elsa.MongoDb.Modules.Runtime;
using ElsaServer.Configuration;
using ElsaServer.Extensions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();

var services = builder.Services;
var configuration = builder.Configuration;

// Get MongoDB settings
var mongoDbSettings = new MongoDbSettings();
builder.Configuration.GetSection("Elsa:MongoDb").Bind(mongoDbSettings);

// Validate settings
if (string.IsNullOrEmpty(mongoDbSettings.ConnectionString))
{
    throw new InvalidOperationException("MongoDB connection string is not configured");
}

// Extract or validate database name
var connectionString = mongoDbSettings.ConnectionString;
var databaseName = mongoDbSettings.DatabaseName;

// If database name is not in connection string, append it
if (!string.IsNullOrEmpty(databaseName) && !connectionString.Contains($"/{databaseName}"))
{
    connectionString = connectionString.TrimEnd('/') + "/" + databaseName;
}

services
    .AddElsa(elsa => elsa
        .UseIdentity(identity =>
        {
            identity.TokenOptions = options => options.SigningKey = "your-secure-signing-key-here-minimum-32-characters";
            identity.UseAdminUserProvider();
        })
        .UseDefaultAuthentication()
        // Use MongoDB with connection string that includes database name
        .UseMongoDb(connectionString)
        // Configure workflow management to use MongoDB
        .UseWorkflowManagement(management =>
        {
            management.UseMongoDb();
        })
        // Configure workflow runtime to use MongoDB
        .UseWorkflowRuntime(runtime =>
        {
            runtime.UseMongoDb();
        })
        .UseScheduling()
        .AddJavaScriptAndLiquid(configuration)
        .UseHttp(http => http.ConfigureHttpOptions = options => configuration.GetSection("Http").Bind(options))
        .UseWorkflowsApi()
        .AddActivitiesFrom<Program>()
        .AddWorkflowsFrom<Program>()
    );

// Add CORS
services.AddCors(cors => cors.AddDefaultPolicy(policy =>
    policy.AllowAnyHeader()
          .AllowAnyMethod()
          .AllowAnyOrigin()
          .WithExposedHeaders("*")));

services.AddRazorPages(options =>
    options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseRouting();
app.UseCors();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi();
app.UseWorkflows();
app.MapFallbackToPage("/_Host");

app.Run();