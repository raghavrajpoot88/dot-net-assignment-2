using ChatApp.DomainModel;
using ChatApp.DomainModel.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApp
{

    public class CustomMiddle
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        //private readonly ILogger<CustomMiddle> _logger;, ILogger<CustomMiddle> logger
        //private readonly ApplicationDbContext _DbContext; ,  ApplicationDbContext applicationDbContext

        public CustomMiddle(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
            //_logger = logger;

            //.CreateLogger("CustomMiddleware");
        }

        public async Task Invoke(HttpContext httpContext, ILogModelCreator logCreator)
        {
            if (httpContext.Request.HasFormContentType)
            {
                var form = await httpContext.Request.ReadFormAsync();

                if (form.ContainsKey("Password"))
                {
                    form = new FormCollection(form.Where(x => x.Key != "Password").ToDictionary(k => k.Key, v => v.Value));
                    httpContext.Request.Form = form;
                }
            }
            
            //_logger.LogInformation("Custom Middleware Initiate");
            //Get username from claim
            var userEmail = httpContext.User;
            var currentUser=userEmail.FindFirst(ClaimTypes.Email);
            var userEmailDetail = currentUser?.Value;
            //_logger.LogInformation($"{userEmailDetail}");
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                RequestLog log = new RequestLog();

                log.RequestDateTimeUtc = DateTime.Now;
                HttpRequest httpRequest = httpContext.Request;

                //log 
                log.LogId = Guid.NewGuid().ToString();
                log.TraceId = httpContext.TraceIdentifier;
                var ip = httpRequest.HttpContext.Connection.RemoteIpAddress;
                log.ClientIp = ip == null ? null : ip.ToString();

                //request
                log.RequestBody = await ReadBodyFromRequest(httpRequest);
                //username
                log.Username = userEmailDetail;

                var jsonString = logCreator.LogString(); /*log json*/
                dbContext.requestlogs.Add(log);
                await dbContext.SaveChangesAsync();
            }
            //await _DbContext.requestlogs.AddAsync(log);
            //await _DbContext.SaveChangesAsync();

            //_logger.LogTrace(jsonString);
            //_logger.LogInformation(logCreator.LogString());
            //_logger.LogWarning(jsonString);
            //_logger.LogCritical(logCreator.LogString());

            await _next(httpContext);



        }
            private async Task<string> ReadBodyFromRequest(HttpRequest request)
            {
                // Ensure the request's body can be read multiple times 
                // (for the next middlewares in the pipeline).
                request.EnableBuffering();
                using var streamReader = new StreamReader(request.Body, leaveOpen: true);
                var requestBody = await streamReader.ReadToEndAsync();
                // Reset the request's body stream position for 
                // next middleware in the pipeline.
                request.Body.Position = 0;
                return requestBody;
            }
    }


    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CustomMiddleExtensions
    {
        public static IApplicationBuilder UseCustomMiddle(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomMiddle>();
        }
    }
}
