namespace SmsClientLibrary.Common.Models;

/// <summary>Класс описания ю</summary>
public class Dish
{
    public string Id { get; set; }

    public string Article { get; set; }

    public string Name { get; set; }

    public double Price { get; set; }

    public bool IsWeighted { get; set; }

    public string FullPath { get; set; }

    public List<string> Barcodes { get; set; } = [];
}

