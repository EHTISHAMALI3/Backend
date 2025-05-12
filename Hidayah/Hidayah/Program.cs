using Hidayah.Application.Services.Decryption;
using Hidayah.Application.Services.Encryption;
using Hidayah.CustomMiddlewares;

//using Hidayah.CustomMiddlewares;
using Hidayah.Domain.Models;
using Hidayah.Infrastrcture.AppDbContext;
using Hidayah.Infrastrcture.ServicesRegistry;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
//using static Dapper.SqlMapper;

var builder = WebApplication.CreateBuilder(args);

string environment = builder.Configuration["ConnectTo"]!;

string connectionString = builder.Configuration["ConnectionStrings:" + environment]!;

string privateKey = builder.Configuration["Decryption:PrivateKey"]!;
string publicKey = builder.Configuration["Encryption:PublicKey"]!;


#region Cores Policies
builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
           .WithExposedHeaders("Authorization"); // 👈 THIS IS REQUIRED
}));

#endregion

// Add services to the container.


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hidayah Web API", Version = "v1" });

    // Define security scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your token."
    });

    // Apply security globally
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});


// Add Authorization services
builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection String not found!"), b => b.MigrationsAssembly("Hidayah")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//*********************Custom Reset password link Confirmation************************
builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
           o.TokenLifespan = TimeSpan.FromMinutes(90));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
            ValidAudience = builder.Configuration["Jwt:ValidAudience"]
        };
    });



builder.Services.AddSingleton(new DecryptionService(privateKey));
builder.Services.AddSingleton(new EncryptionService(publicKey));
//builder.Services.AddHttpClient<OpenAIService>();


builder.Services.AddInfrastructure();

builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.Configure<ApiBehaviorOptions>(options =>
//{
//    options.SuppressModelStateInvalidFilter = true;
//});
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

// If using IIS:
builder.Services.Configure<IISServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Already added in your code
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hidayah Web API"));
}
else
{
    app.UseExceptionHandler("/Home/Error");
    //app.UseHsts();
}


// Add a default route to display API status
app.MapGet("/", async context =>
{
    try
    {
        // Systematic project name
        var projectName = "Hidayah Web API";

        // Check database connectivity
        string dbStatus;
        try
        {
            string environment = builder.Configuration["ConnectTo"]!;

            string connectionString = builder.Configuration["ConnectionStrings:" + environment]!;

            // Simulate a database connection check
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(); // Attempt to open the connection
                dbStatus = "Connected"; // If successful, set status as connected
            }
        }
        catch (Exception dbEx)
        {
            dbStatus = $"Error: {dbEx.Message}";
        }



        // Create HTML for the status page
        var html = $@"
            <html>
                <head><title>{projectName} - Status</title></head>
                <body>
                    <h1>{projectName}</h1>
                    
                    <p><strong>Database Status:</strong> {dbStatus}</p>
                </body>
            </html>";

        // Respond with the HTML content
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(html);
    }
    catch (Exception ex)
    {
        // Handle errors and display a plain-text error message
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync($"An error occurred: {ex.Message}");
    }
});


app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hidayah Web API v1"));
app.UseStaticFiles();

app.UseRouting();
app.UseCors("CorsPolicy");
app.UseMiddleware<JwtMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// Map Fallback
//app.MapFallbackToFile("index.html");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
