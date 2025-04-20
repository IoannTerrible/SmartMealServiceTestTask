using Google.Protobuf.WellKnownTypes;

using Grpc.Net.Client;

using Sms.Test;

using SmsClientLibrary.Common.Clients;
using SmsClientLibrary.Common.Models;

namespace SmsClientLibrary.gRPS.Clients;

/// <summary>gRPC‑клиент.</summary>
public class SMSgRPSClient : ISmsClient
{
    private readonly SmsTestService.SmsTestServiceClient _client;

    public SMSgRPSClient(string grpcServerUrl)
    {
        var channel = GrpcChannel.ForAddress(grpcServerUrl);
        _client = new SmsTestService.SmsTestServiceClient(channel);
    }

    public async Task<GetMenuResponce> GetMenuAsync()
    {
        var grpcResponse = await _client.GetMenuAsync(new BoolValue { Value = true });

        var result = new GetMenuResponce
        {
            Success = grpcResponse.Success,
            ErrorMessage = grpcResponse.ErrorMessage
        };

        if (grpcResponse.Success)
        {
            result.Dishes = [.. grpcResponse.MenuItems
                .Select(x => new Dish
                {
                    Id = x.Id,
                    Article = x.Article,
                    Name = x.Name,
                    Price = x.Price,
                    IsWeighted = x.IsWeighted,
                    FullPath = x.FullPath,
                    Barcodes = [.. x.Barcodes]
                })];
        }

        return result;
    }

    public async Task<SendOrderResponce> SendOrderAsync(Common.Models.Order order)
    {
        var grpcOrder = new Sms.Test.Order { Id = order.OrderId };
        grpcOrder.OrderItems.AddRange(order.MenuItems.Select(i => new Sms.Test.OrderItem
        {
            Id = i.Id,
            Quantity = i.Quantity
        }));

        var grpcResponse = await _client.SendOrderAsync(grpcOrder);

        return new SendOrderResponce
        {
            Success = grpcResponse.Success,
            ErrorMessage = grpcResponse.ErrorMessage
        };
    }
}
