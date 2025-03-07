using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Database Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=newsdb.db"));
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

// Add Identity for Authentication
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add Authentication (JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

// Add Authorization
builder.Services.AddAuthorization();

// Add Controllers
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

// Build Application
var app = builder.Build();

app.UseAuthentication(); // Enable Authentication
app.UseAuthorization(); // Enable Authorization

// Enable Swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

app.MapControllers();
app.Run();
