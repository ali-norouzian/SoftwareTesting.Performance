using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RequestReceiver;

class Consumer
{
    //static readonly Counter MessageCounter = Metrics.CreateCounter("rabbitmq_messages_processed", "Number of messages processed");
    //static readonly Histogram ProcessingTime = Metrics.CreateHistogram("rabbitmq_processing_time", "Time taken to process messages");

    static async Task Main(string[] args)
    {
        //var server = new MetricServer(port: 8080);
        //server.Start();
        //Console.WriteLine("Prometheus metrics available at http://localhost:8080/metrics");

        var factory = new ConnectionFactory() { HostName = "localhost" };
        await using (var connection = await factory.CreateConnectionAsync())
        await using (var channel = await connection.CreateChannelAsync())
        {
            await channel.QueueDeclareAsync(queue: "test_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                //var stopwatch = Stopwatch.StartNew();

                //var body = ea.Body.ToArray();
                //var message = Encoding.UTF8.GetString(body);
                //Console.WriteLine($"[x] Received {message}");

                await Data.IncreaseOneMongoAsync();

                // Simulating message processing delay
                //System.Threading.Thread.Sleep(50);

                //stopwatch.Stop();
                //ProcessingTime.Observe(stopwatch.ElapsedMilliseconds);
                //MessageCounter.Inc();
            };

            await channel.BasicConsumeAsync(queue: "test_queue", autoAck: true, consumer: consumer);

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}



