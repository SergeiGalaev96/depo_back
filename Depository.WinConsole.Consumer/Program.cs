using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Depository.WinConsole.Consumer
{
    class Program
    {
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.ReceiveEndpoint("event-listener_skipped", e =>
                {
                    e.Consumer<EventConsumer>();
                });
            });

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);
            try
            {
                Console.WriteLine("Press enter to exit");

                await Task.Run(() => Console.ReadLine());
            }
            finally
            {
                await busControl.StopAsync();
            }
        }

        class EventConsumer :
            IConsumer<ValueEntered>
        {
            public async Task Consume(ConsumeContext<ValueEntered> context)
            {
                Console.WriteLine("Value: {0}", context.Message.Value);
            }
        }
    }
}
