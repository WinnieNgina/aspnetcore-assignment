using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models.ViewModels;
public class CheckInViewModel
{
    [Key]
    public string Name { get; set; }
    public int ServiceId { get; set; }
}
