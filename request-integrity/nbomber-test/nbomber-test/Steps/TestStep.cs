using System.Net;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using PerfTest;

namespace nbomber_test.Steps;

public class TestStep
{
    public static IStep GetStep(IClientFactory<HttpClient> httpFactory)
    {
        var step = Step.Create(
            "step",
            clientFactory: httpFactory,
            async context =>
            {
                var key = Guid.NewGuid();
                var value = Guid.NewGuid();

                var addResult = await Requester.AddValueRequest(key.ToString(), value.ToString());
                if (addResult.StatusCode != HttpStatusCode.OK)
                {
                    return addResult.ToNBomberResponse();                    
                }

                var retrieveResult = await Requester.RetrieveValueRequest(key.ToString());
                if (retrieveResult.StatusCode == HttpStatusCode.OK &&
                    retrieveResult.Content.ReadAsStringAsync().Result != value.ToString())
                {
                    return Response.Fail("Stored value mismatch");
                }

                return retrieveResult.ToNBomberResponse();
            }, 
            timeout: TimeSpan.FromSeconds(2));

        return step;
    }
}
