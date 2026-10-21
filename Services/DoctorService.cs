using Microsoft.Extensions.Caching.Memory;

namespace HospitalAppointmentMVC.Services;

public class DoctorService(IMemoryCache cache)
{
    private const string CacheKey = "hospital_doctor_list";

    public List<Doctor> GetDoctors()
    {
        return cache.GetOrCreate(CacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3);
            entry.SlidingExpiration = TimeSpan.FromMinutes(1);

            return new List<Doctor>
            {
                new() { DoctorId = 1, DoctorName = "Dr. Aditi Sharma", Specialization = "Cardiology", Experience = 12, ConsultationFee = 800m },
                new() { DoctorId = 2, DoctorName = "Dr. Rohan Mehta", Specialization = "Dermatology", Experience = 8, ConsultationFee = 600m },
                new() { DoctorId = 3, DoctorName = "Dr. Neha Patel", Specialization = "Pediatrics", Experience = 10, ConsultationFee = 500m },
                new() { DoctorId = 4, DoctorName = "Dr. Arjun Shah", Specialization = "Orthopedics", Experience = 15, ConsultationFee = 900m },
                new() { DoctorId = 5, DoctorName = "Dr. Kavya Desai", Specialization = "Neurology", Experience = 7, ConsultationFee = 750m }
            };
        })!;
    }

    public Doctor? GetDoctorById(int id) => GetDoctors().FirstOrDefault(d => d.DoctorId == id);
}
