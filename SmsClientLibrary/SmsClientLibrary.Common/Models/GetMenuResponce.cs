namespace SmsClientLibrary.Common.Models;

public class GetMenuResponce
{
    public bool Success { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;

    public List<Dish> Dishes { get; set; } = new();
}
