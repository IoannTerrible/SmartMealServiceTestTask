namespace SmsClientLibrary.Common.Models;

/// <summary>Оформленный заказ</summary>
public class Order
{
    public string OrderId { get; set; }

    public List<OrderItem> MenuItems { get; set; } = [];
}
