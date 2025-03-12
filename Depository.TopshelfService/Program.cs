using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Depository.TopshelfService
{
    class Program
    {
        public static int Main()
        {
            return (int)HostFactory.Run(cfg => cfg.Service(x => new EventConsumerService()));
        }

        class EventConsumerService :
            ServiceControl
        {
            IBusControl _bus;

            public bool Start(HostControl hostControl)
            {
                _bus = ConfigureBus();
                _bus.Start(TimeSpan.FromSeconds(10));

                return true;
            }

            public bool Stop(HostControl hostControl)
            {
                _bus?.Stop(TimeSpan.FromSeconds(30));

                return true;
            }

            IBusControl ConfigureBus()
            {
                return Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host("192.168.2.185","/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    //cfg.ReceiveEndpoint("event-listener", e =>
                    //{
                    //    e.Consumer<Instruc>();
                    //});
                    //return Bus.Factory.CreateUsingRabbitMq(cfg =>
                    //{
                    //    cfg.ReceiveEndpoint("event-listener", e =>
                    //    {
                    //        e.Consumer<EventConsumer>();
                    //    });
                    //});

                });

                //return Bus.Factory.CreateUsingRabbitMq(cfg =>
                //{

                //    cfg.ReceiveEndpoint("MskDotNet.HelloWorld.InputQueue", e =>
                //    {
                //        e.Consumer<SayHiCommandHandler>();
                //    });
                //});
            }

            class SayHiCommandHandler :
            IConsumer<SayHi>
            {
                public async Task Consume(ConsumeContext<SayHi> context)
                {
                    Console.WriteLine("Value: {0}", context.Message.Name);
                }
            }
        }
    }
}
