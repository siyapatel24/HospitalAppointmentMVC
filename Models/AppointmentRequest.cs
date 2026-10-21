using System.ComponentModel.DataAnnotations;

namespace HospitalAppointmentMVC.Models;

public class AppointmentRequest
{
    [Required]
    [StringLength(80, MinimumLength = 2)]
    public string PatientName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime AppointmentDate { get; set; } = DateTime.Today.AddDays(1);

    public int DoctorId { get; set; }
}
