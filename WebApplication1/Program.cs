using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApplication1.Data;          
using WebApplication1.Models;         
using WebApplication1.Services;       
using WebApplication1.Interfaces;     

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=newsdb.db"));


builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
});


builder.Services.AddAuthentication(options => 
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"] ?? 
            "defaultkeywhichmustbelongerthan64bytestoworkcorrectlywiththehmacshasignaturealgorithm123456!")),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"] ?? "webapi",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"] ?? "webclient",
        ClockSkew = TimeSpan.Zero
    };
});


builder.Services.AddScoped<IAuthorizationHandler, ArticleAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CommentAuthorizationHandler>();
builder.Services.AddScoped<ICustomAuthorizationService, AuthorizationService>();

builder.Services.AddScoped<TokenService>();


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanCreateArticle", policy =>
        policy.RequireRole("Editor", "Writer"));
        
    options.AddPolicy("CanEditArticle", policy =>
        policy.RequireRole("Editor"));
        
    options.AddPolicy("CanCreateComment", policy =>
        policy.RequireRole("Editor", "Writer", "Subscriber"));
});


builder.Services.AddControllers();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "News API", Version = "v1" });
    
    
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


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "News API V1");
    c.RoutePrefix = string.Empty; 
});

// HTTPS Redirection
app.UseHttpsRedirection();

// Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Add seed data if needed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        
        context.Database.EnsureCreated();
        
       
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        
        
        await CreateRoles(roleManager);
        
        
        
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing roles.");
    }
}

app.Run();

// Role initialization method
async Task CreateRoles(RoleManager<IdentityRole> roleManager)
{
    string[] roleNames = { "Editor", "Writer", "Subscriber", "Guest" };
    
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}
