namespace Queue_Management_System.Models;
/// <summary>
/// Represents the location or department where services are provided. 
/// It's linked to customers and service providers, indicating where services are offered.
/// </summary>
public class ServicePointModel
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string? Description { get; set; }
    public ICollection<CustomerModel>? Customers { get; set; }
}
