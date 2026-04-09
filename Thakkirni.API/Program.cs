using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
<<<<<<< HEAD
using Thakkirni.API.Application.Interfaces;
using Thakkirni.API.Application.Services;
using Thakkirni.API.Data;
using Thakkirni.API.Infrastructure.Identity;
using Thakkirni.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
=======
using Thakkirni.API.Data;
using Thakkirni.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

<<<<<<< HEAD
// FORCE a single known database connection for debugging (bypass appsettings)
var forcedConnection = "Server=(localdb)\\MSSQLLocalDB;Database=ThakkirniDb;Trusted_Connection=True;";
Console.WriteLine($"FORCED DB: ThakkirniDb on (localdb)\\MSSQLLocalDB (connection used for DbContext)");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        forcedConnection,
        sql =>
        {
            sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            sql.CommandTimeout(30);
        }));

var cs = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"USING DB: {cs}");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", corsBuilder =>
    {
        corsBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IJobTitleService, JobTitleService>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");
=======
// Background service: auto-mark tasks as OVERDUE when dueDate passes
builder.Services.AddHostedService<OverdueStatusService>();

// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

<<<<<<< HEAD
app.UseMiddleware<GlobalExceptionMiddleware>();

Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, "wwwroot", "uploads", "chat"));

=======
// Configure the HTTP request pipeline.
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

<<<<<<< HEAD
app.UseCors("AllowAngular");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
=======
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
<<<<<<< HEAD
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
=======
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
    DbSeeder.Seed(context);
}

app.Run();
