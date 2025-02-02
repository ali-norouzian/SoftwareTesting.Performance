using System.Collections.Concurrent;
using System.Text;
using NBomber.CSharp;
using RabbitMQ.Client;

class Program
{
    static async Task Main(string[] args)
    {

        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = await factory.CreateConnectionAsync();
        var channelPool = new ConcurrentBag<IChannel>();

        // Create a pool of 1000 channels
        for (int i = 0; i < 1000; i++)
        {
            channelPool.Add(await connection.CreateChannelAsync());
        }

        var scenario = Scenario.Create("RabbitMQ Load Test", async context =>
            {
                if (!channelPool.TryTake(out var channel))
                {
                    return Response.Fail();
                }

                try
                {
                    var message = "Test Message";
                    var body = Encoding.UTF8.GetBytes(message);

                    await channel.BasicPublishAsync("", "test_queue", false, body, default);
                }
                finally
                {
                    channelPool.Add(channel);
                }


                //await Data.IncreaseOneSqlAsync();


                return Response.Ok();
            })
            .WithLoadSimulations(new[]
            {
                Simulation.Inject(rate: 1000, interval: TimeSpan.FromMilliseconds(1000), during: TimeSpan.FromMinutes(1))
            });


        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();


        foreach (var channel in channelPool)
        {
            await channel.CloseAsync();

        }
        await connection.CloseAsync();
    }
}
