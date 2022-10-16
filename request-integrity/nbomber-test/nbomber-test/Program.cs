using nbomber_test.Steps;
using NBomber.CSharp;

namespace PerfTest
{
    class Program
    {
        static void Main(string[] args)
        {   
            var httpFactory = ClientFactory.Create(
                name: "http_factory",                         
                clientCount: 1,
                initClient: (number,context) => Task.FromResult(Requester.PrepareHttpClient())
            );

            var scenario = ScenarioBuilder
                .CreateScenario("step1", TestStep.GetStep(httpFactory))
                .WithWarmUpDuration(TimeSpan.FromSeconds(1))
                .WithLoadSimulations(new []{
                    Simulation.KeepConstant(copies: 4, during: TimeSpan.FromSeconds(15))
                });

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }
    }
}
