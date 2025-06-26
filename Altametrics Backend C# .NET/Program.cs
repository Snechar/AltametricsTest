using Altametrics_Backend_C__.NET.Data;
using Altametrics_Backend_C__.NET.Database;
using Altametrics_Backend_C__.NET.Mapper;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Altametrics_Backend_C__.NET.Services;
using AspNetCoreRateLimit;





var builder = WebApplication.CreateBuilder(args);

//load environment variables from .env file
Env.Load();


//setting up connection string
var connString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                 $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                 $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                 $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                 $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";


//setting up JWT authentication
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
if (builder.Environment.IsEnvironment("Testing"))
{
    jwtKey ??= "TestSecretKeyThatIsLongEnough123!";
}
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? throw new InvalidOperationException("JWT Key is missing"))),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


// Add services to the container.
builder.Services.AddScoped<IAuditLogger, AuditLogger>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHostedService<ReminderService>();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


// Update the Swagger configuration to fix the error  
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Altametrics API V1", Version = "v1" });



    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT in the format: Bearer {token}"
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
           new string[] {}
       }
   });
});  

//adding db context
if (builder.Environment.EnvironmentName != "Testing")
{
    builder.Services.AddDbContext<AppDBContext>(options =>
        options.UseNpgsql(connString));
    builder.Services.AddHostedService<ReminderService>();
}

//automapper because we need to map our models to database entities and vice versa
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));




var app = builder.Build();
if (builder.Environment.EnvironmentName != "Testing")
{
    //ensure the database exists, if not create it
    DBInitializer.EnsureDatabaseExists(connString);

    // Initialize the database schema if it hasn't been initialized yet
    DBInitializer.RunInitSchema(app);
}

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Altametrics API V1");
        c.SwaggerEndpoint("/swagger/Events - Authorized/swagger.json", "Events - Authorized");
        c.SwaggerEndpoint("/swagger/Events - Anonymous/swagger.json", "Events - Anonymous");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseIpRateLimiting();

app.MapControllers();

app.Run();
public partial class Program { }
