using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Transport.Controllers;
using Transport.Data;
using Transport.Repositories.Implimentation;
using Transport.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    //c.SwaggerDoc("v1", new OpenApiInfo { Title = "Transport API", Version = "v1" });
    //c.SchemaFilter<FileUploadSchemaFilter>();
});

builder.Services.AddDbContext<TransportDbContext>(Options =>
{
    var connectionString = builder.Configuration.GetConnectionString("TransportDb");

    if (connectionString == null)
    {
        throw new InvalidOperationException("Connection string is null");
    }
    Options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IUserRegistration, UserRegistration>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // Enable Swagger metadata endpoint
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transport API V1"); // Swagger endpoint configuration
        c.RoutePrefix = "swagger"; // Swagger UI available at /swagger
    });
}
if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")))
{
    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
}

// Enable Static Files Serving (default wwwroot folder)
app.UseStaticFiles();


app.UseHttpsRedirection();
app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowAnyOrigin();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
