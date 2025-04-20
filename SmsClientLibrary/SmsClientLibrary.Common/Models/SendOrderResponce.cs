namespace SmsClientLibrary.Common.Models;

/// <summary>DTO обёртка для ответа на запрос SetOrder у <see cref="SmsClientLibrary"/></summary>
public class SendOrderResponce
{
    public bool Success { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;
}
