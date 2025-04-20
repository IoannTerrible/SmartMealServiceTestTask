using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using SmsClientLibrary.Common.Clients;
using SmsClientLibrary.Common.Models;
using SmsClientLibrary.Database;
using SmsClientLibrary.Database.Entities;
using SmsClientLibrary.gRPS.Clients;
using System.Globalization;

namespace SmsClientLibrary.ConsoleClient;

internal class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var logFile = config["Logging:LogFilePath"]!;

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(
                path: config["Logging:LogFilePath"]!,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day
            )
            .CreateLogger();



        var grpcUrl = config["Service:GrpcUrl"]!;
        var connectionString = config.GetConnectionString("Postgres")!;

        var optionsBuilder = new DbContextOptionsBuilder<SmsDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        using var db = new SmsDbContext(optionsBuilder.Options);
        db.Database.EnsureCreated();

        var client = new SMSgRPSClient(grpcUrl);

        Log.Information("Запрос меню...");
        var menuResult = await client.GetMenuAsync();
        if (!menuResult.Success)
        {
            Log.Error("Ошибка при получении меню: {Error}", menuResult.ErrorMessage);
            return;
        }

        foreach (var dish in menuResult.Dishes)
        {
            if (!await db.Dishes.AnyAsync(d => d.Id == dish.Id))
            {
                db.Dishes.Add(new DishEntity
                {
                    Id = dish.Id,
                    Article = dish.Article,
                    Name = dish.Name,
                    Price = dish.Price
                });
            }
        }
        await db.SaveChangesAsync();

        Log.Information("Список блюд:");
        foreach (var d in menuResult.Dishes)
            Log.Information("{Name} – {Article} – {Price}", d.Name, d.Article, d.Price.ToString(CultureInfo.InvariantCulture));

        List<Dish> available = menuResult.Dishes;
        Order order = new() { OrderId = Guid.NewGuid().ToString() };

        while (true)
        {
            Log.Information("Введите позиции (Код:Кол-во;Код2:Кол-во;...):");
            string? line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(';', StringSplitOptions.RemoveEmptyEntries);
            bool ok = true;

            foreach (var part in parts)
            {
                var kv = part.Split(':', 2);
                if (kv.Length != 2
                    || !double.TryParse(kv[1].Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var qty)
                    || qty <= 0
                    || available.All(d => d.Article != kv[0]))
                {
                    Log.Warning("Некорректная запись '{Part}'", part);
                    ok = false;
                    break;
                }
            }

            if (!ok) continue;

            foreach (var part in parts)
            {
                var kv = part.Split(':', 2);
                var dish = available.First(d => d.Article == kv[0]);
                var qty = double.Parse(kv[1].Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture);
                order.MenuItems.Add(new OrderItemImp { Id = dish.Id, Quantity = qty });
            }
            break;
        }

        Log.Information("Отправка заказа {OrderId}...", order.OrderId);
        var sendResult = await client.SendOrderAsync(order);
        if (sendResult.Success) Log.Information("УСПЕХ");
        else Log.Error("Ошибка при отправке: {Error}", sendResult.ErrorMessage);
    }
}
