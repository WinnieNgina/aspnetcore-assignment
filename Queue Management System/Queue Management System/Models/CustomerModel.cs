namespace Queue_Management_System.Models;
/// <summary>
/// Represents the individual or entity receiving a service.
/// </summary>
public class CustomerModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ServiceType { get; set; }
    public string ServedBy { get; set; }
    public int TicketNumber { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime ServiceStartTime { get; set; }
    public DateTime checkout { get; set; }
    public int ServicePointID { get; set; }
    /// <summary>
    /// The ServicePointID links the customer to the specific service point they are associated with.
    /// </summary>
    public ServicePointModel ServicePoint { get; set; }
}

