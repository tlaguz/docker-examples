using System.Net;
using StackExchange.Redis;

namespace request_integrity;

public class RetrieveValueRequest
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly HttpContext _httpContext;

    public RetrieveValueRequest(
        IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }
    
    public object Process(string key, HttpContext context)
    {
        if (!_connectionMultiplexer.IsConnected)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return "";
        }

        var redis = _connectionMultiplexer.GetDatabase();

        string result = redis.StringGet(key);
        if (result == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return "";
        }

        context.Response.StatusCode = (int)HttpStatusCode.OK;
        return result;
    }
}
