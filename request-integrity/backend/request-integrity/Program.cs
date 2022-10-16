using System.Text;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace request_integrity;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.redis.json");

        var redis = ConnectionMultiplexer.Connect(builder.Configuration["RedisConnectionString"]);
        builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
        builder.Services.AddScoped<AddValueRequest>();
        builder.Services.AddScoped<RetrieveValueRequest>();
        
        var app = builder.Build();

        app.MapPost("/value/{key}", (string key, HttpContext context) =>
        {
            Task<string> value;
            using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
            {  
                value = reader.ReadToEndAsync();
            }
            
            var r = context.RequestServices.GetService<AddValueRequest>();
            return r.Process(key, value.Result, context);
        });
        
        app.MapGet("/value/{key}", (string key, HttpContext context) =>
        {
            var r = context.RequestServices.GetService<RetrieveValueRequest>();
            return r.Process(key, context);
        });
        
        app.Run();
    }
}
