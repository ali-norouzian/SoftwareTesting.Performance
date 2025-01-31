using NBomber.CSharp;
using RabbitMQ.Client;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        var scenario = Scenario.Create("RabbitMQ Load Test", async context =>
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            await using (var connection = await factory.CreateConnectionAsync())
            await using (var channel = await connection.CreateChannelAsync())
            {
                await channel.QueueDeclareAsync(queue: "test_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                var message = "Test Message";
                var body = Encoding.UTF8.GetBytes(message);
                await channel.BasicPublishAsync("", "test_queue", false, body, default);
            }

            //await Data.IncreaseOneMongoAsync();

            return Response.Ok();
        })
        .WithLoadSimulations(new[]
        {
            Simulation.Inject(rate: 1000, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1))
        });

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
