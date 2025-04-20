using SmsClientLibrary.Common.Models;

namespace SmsClientLibrary.Common.Clients;

/// <summary>Клиент для отправки смс запросов.</summary>
public interface ISmsClient
{
    /// <summary>Получить меню блюд.</summary>
    /// <returns>Список <see cref="Dish"/>.</returns>
    Task<GetMenuResponce> GetMenuAsync();

    /// <summary>Отправить заказ.</summary>
    /// <param name="order">DTO заказа</param>
    /// <returns>Успешность операции.</returns>
    Task<SendOrderResponce> SendOrderAsync(Order order);
}
