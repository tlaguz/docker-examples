using System.Net;
using StackExchange.Redis;

namespace request_integrity;

public class AddValueRequest
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public AddValueRequest(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }
    
    public object Process(string key, string value, HttpContext context)
    {
        if (!_connectionMultiplexer.IsConnected)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return "";
        }

        var redis = _connectionMultiplexer.GetDatabase();

        string result = redis.StringGet(key);
        if (result != null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return "";
        }

        redis.StringSet(key, value, TimeSpan.FromSeconds(60));

        context.Response.StatusCode = (int)HttpStatusCode.OK;
        return "";
    }
}
