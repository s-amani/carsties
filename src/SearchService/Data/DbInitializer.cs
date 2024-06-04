using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Data;

public class DbInitializer
{
    public static async Task Initialize(WebApplication app)
    {
        await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("Default")));

        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();

        var count = await DB.CountAsync<Item>();

        if (count > 0)
          return;

        Console.WriteLine("--> No data, attempting to seed the data");

        using var scope = app.Services.CreateScope();

        var httpClient = scope.ServiceProvider.GetService<AuctionServiceHttpClient>();

        var result = await httpClient.GetItemsForSearchDB();

        Console.WriteLine($"--> {result.Count} items received from the Auction Service");

        // var itemData = await File.ReadAllTextAsync("Data/auction.json");

        // var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};
        
        // var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

        await DB.SaveAsync(result);
    }
}
