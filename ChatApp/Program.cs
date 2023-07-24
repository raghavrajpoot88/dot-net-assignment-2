//using ChatApp;
using ChatApp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Context;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using ChatApp.Hubs;
using ChatApp.DomainModel;
using ChatApp.DomainModel.Repo;
using ChatApp.DomainModel.Repo.Interfaces;
using ChatApp.MiddleLayer.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddIdentity<IdentityUser,IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
    }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
     
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddScoped<IUser ,UserRepository>();
builder.Services.AddScoped<IMessages, MessagesRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
    });
builder.Services.AddHttpLogging(httpLogging =>
{
    httpLogging.LoggingFields = HttpLoggingFields.All;
});
builder.Host.UseSerilog((hostingContext, LoggerConfig) =>
{
    LoggerConfig.ReadFrom.Configuration(hostingContext.Configuration);
});


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
    policy =>
    {
        policy.AllowAnyHeader().WithOrigins("http://localhost:4200").AllowAnyMethod();
    });
});
builder.Services.AddSignalR();


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
app.UseSerilogRequestLogging();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.UseCustomMiddle();

app.Run();
