using Backend.Models;
using Backend.Repositories.BookRepository;
using Backend.Repositories.ShelterRepository;
using Backend.Repositories.UserRepository;
using Backend.Services.BookService;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Backend.Services.ShelterService;
using Microsoft.EntityFrameworkCore;

using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin() // Adresy Swaggera
       .AllowAnyMethod()
       .AllowAnyHeader();
      // .AllowCredentials();
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<LibraryContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection1"));
});
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IShelterRepository, ShelterRepository>();
builder.Services.AddScoped<IShelterService, ShelterService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IBorrowRepository, BorrowRepository>();
builder.Logging.ClearProviders(); // Wyczyœæ domyœlnych dostawców logów
builder.Logging.AddConsole();     // Dodaj logowanie do konsoli
builder.Logging.SetMinimumLevel(LogLevel.Debug);


/*    var firebaseApp= FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromFile("F:\\wat\\9 sem\\Pz\\Backend\\pz-auth-firebase-adminsdk-5h2gk-45d7aade3e.json")
    });
*/
/*builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions =>
    {
        //jwtOptions.RequireHttpsMetadata = false;
        jwtOptions.Authority = "https://securetoken.google.com/pz-auth-43fb0";
        jwtOptions.Audience = "pz-auth-43fb0";
        jwtOptions.TokenValidationParameters.ValidIssuer = "https://securetoken.google.com/pz-auth-43fb0";
        jwtOptions.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError($"B³¹d uwierzytelniania: {context.Exception.Message}");
                return Task.CompletedTask;
            },

            *//*OnTokenValidated = context =>
            {
                var jwtToken = context.SecurityToken as JwtSecurityToken;
                if (jwtToken != null)
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Token poprawnie zweryfikowany.");

                    // Dodatkowa logika walidacji tokenu
                    // Mo¿esz sprawdziæ rêcznie podpis i inne elementy tokenu
                }
                return Task.CompletedTask;
            },
*//*
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning($"Wyzwanie JWT: {context.ErrorDescription}");
                return Task.CompletedTask;
            }
        };

    });
*/




builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });

});





var app = builder.Build();

app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
/*app.UseAuthentication();
app.UseAuthorization();*/

app.MapControllers();

app.Run();
