//using ChatApp;
using ChatApp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.Filters;
using System.Text;
using ChatApp.Hubs;
using ChatApp.DomainModel;
using ChatApp.DomainModel.Repo;
using ChatApp.DomainModel.Repo.Interfaces;
using ChatApp.MiddleLayer.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection")
        , ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")));
});

builder.Services.AddIdentity<IdentityUser,IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
     
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddScoped<IUser ,UserRepository>();
builder.Services.AddScoped<IMessagesService, MessagesService>();
builder.Services.AddScoped<IMessages, MessagesRepository>();
builder.Services.AddScoped<IRequestLogsService, RequestLogsService>();
builder.Services.AddScoped<IRequestLog, RequestLogRepository>();
builder.Services.AddScoped<ILogModelCreator, LogModelCreator>();
//builder.Services.AddSingleton<IRequestLogger, RequestLogger>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer=false,
            ValidateAudience=false
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for our hub...
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/hub")))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
                //// After successful token validation, retrieve the user's ID from the 'user' object.
                //var userIdHub = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                //// Add the 'NameIdentifier' claim with the user's ID to the ClaimsIdentity.
                //var identity = context.Principal.Identity as ClaimsIdentity;
                //identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userIdHub));
            },
        };
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Google:ClientId"];
        options.ClientSecret = builder.Configuration["Google:ClientSecret"];
    });


builder.Services.AddHttpLogging(httpLogging =>
{
    httpLogging.LoggingFields = HttpLoggingFields.All;
});
////builder.Host.UseSerilog((hostingContext, LoggerConfig) =>
//{
//    LoggerConfig.ReadFrom.Configuration(hostingContext.Configuration);
//});


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
    policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .WithOrigins("http://localhost:4200")
              .AllowCredentials(); // Allow credentials
    });
});
builder.Services.AddSignalR();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Doing Standard Authorization header using the Bearer Scheme (\"bearer{token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey

    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpLogging();

app.UseHttpsRedirection();
//app.UseSerilogRequestLogging();

app.UseAuthentication();

app.UseAuthorization();

//app.UseAppException();

app.UseCors();

app.MapControllers();
//app.UseLogging();

app.UseMiddleware<CustomMiddle>();


app.MapHub<ChatHub>("/hub");

app.Run();
