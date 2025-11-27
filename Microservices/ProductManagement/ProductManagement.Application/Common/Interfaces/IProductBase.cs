namespace ProductManagement.Application.Common.Interfaces;

public interface IProductBase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool Availability { get; set; }
}