using Backend.Models;
using Backend.Repositories.ShelterRepository;
using Backend.Repositories.UserRepository;
using Backend.Services.ShelterService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()    // Allows requests from any origin
              .AllowAnyMethod()    // Allows any HTTP method (GET, POST, etc.)
              .AllowAnyHeader();   // Allows any headers
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

// Rejestracja repozytorium
builder.Services.AddScoped<IShelterRepository, ShelterRepository>();
// Rejestracja serwisu
builder.Services.AddScoped<IShelterService, ShelterService>();

var app = builder.Build();

app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
