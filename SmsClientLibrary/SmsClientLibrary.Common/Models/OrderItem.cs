namespace SmsClientLibrary.Common.Models;

/// <summary>Обьект заказа</summary>
/// <remarks>Сделал абстрактным.</remarks>
public abstract class OrderItem
{
    public string Id { get; set; }

    public double Quantity { get; set; }
}
