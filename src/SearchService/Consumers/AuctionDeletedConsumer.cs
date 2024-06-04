using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
{
    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {
        var item = DB.Queryable<Item>().FirstOrDefault(x => x.ID == context.Message.Id);
        var result = await item.DeleteAsync();
    }
}
