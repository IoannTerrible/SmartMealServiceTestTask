namespace SmsClientLibrary.Database.Entities;

public class DishEntity
{
    public string Id { get; set; } = null!;

    public string Article { get; set; } = null!;

    public string Name { get; set; } = null!;

    public double Price { get; set; }

    public bool IsWeighted { get; set; }

    public string FullPath { get; set; } = null!;

    public List<string> Barcodes { get; set; } = [];
}
