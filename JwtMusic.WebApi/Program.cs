using System.Text;
using JwtMusic.WebApi.Context;
using JwtMusic.WebApi.Entities;
using JwtMusic.WebApi.Services.LoginServices;
using JwtMusic.WebApi.Services.RegisterServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var jwt = builder.Configuration.GetSection("JwtSettings");

var databasePath = Path.Combine(builder.Environment.ContentRootPath, "JwtMusic.db");
builder.Services.AddDbContext<JwtContext>(options => options.UseSqlite($"Data Source={databasePath}"));
builder.Services.AddIdentityCore<AppUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireDigit = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<JwtContext>()
    .AddSignInManager();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "JwtMusic API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT",
        In = ParameterLocation.Header, Description = "Girişten dönen JWT token"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = Array.Empty<string>()
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<JwtContext>();
    await context.Database.EnsureCreatedAsync();
    var connection = context.Database.GetDbConnection();
    await connection.OpenAsync();
    await using (var check = connection.CreateCommand())
    {
        check.CommandText = "SELECT COUNT(*) FROM pragma_table_info('Songs') WHERE name = 'StoreUrl'";
        if (Convert.ToInt32(await check.ExecuteScalarAsync()) == 0)
        {
            await using var alter = connection.CreateCommand();
            alter.CommandText = "ALTER TABLE Songs ADD COLUMN StoreUrl TEXT NOT NULL DEFAULT ''";
            await alter.ExecuteNonQueryAsync();
        }
    }
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    await SeedData.InitializeAsync(context, userManager);
}
app.Run();

public partial class Program { }
