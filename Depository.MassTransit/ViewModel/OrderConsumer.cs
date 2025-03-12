using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Depository.MassTransit.ViewModel
{
    public class OrderConsumer : IConsumer<Order>
    {
        public async Task Consume(ConsumeContext<Order> context)
        {
            var data = context.Message;
            // message received.
        }
    }
}
