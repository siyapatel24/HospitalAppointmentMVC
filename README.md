# Hospital Appointment Management System

An ASP.NET Core MVC demo project implementing the requirements in Task A.

## Requirements covered

1. **Doctor model and extension-style calculation method**
   - `Doctor` has `DoctorId`, `DoctorName`, `Specialization`, `Experience`, and `ConsultationFee`.
   - `CalculateTotalCharge()` adds a ₹100 service charge.
   - Doctor list and details pages display the base fee and total charge.
2. **Asynchronous web request**
   - `DoctorsController.Posts` asynchronously fetches `https://jsonplaceholder.typicode.com/posts`.
   - `Views/Doctors/Posts.cshtml` displays post ID and title.
3. **State management**
   - Cookie `PatientName` stores the patient's name for 30 days.
   - Session stores `SelectedSpecialization`.
   - Query string carries the doctor ID, for example `/Doctors/Details?id=1`.
   - Appointment form includes a hidden `DoctorId` field.
4. **Caching**
   - Doctor list uses `IMemoryCache` with a 3-minute absolute expiration.
   - Doctor details action has Response Caching enabled for 60 seconds.

## Run locally

1. Install the .NET 8 SDK.
2. Open a terminal in this project folder.
3. Run:

   ```bash
   dotnet restore
   dotnet run
   ```

4. Open the HTTP/HTTPS URL printed in the terminal. The launch profile can be selected in your IDE.

## Notes

- The doctor list is sample data held in memory and is not a persistent database.
- Posts require an internet connection to reach JSONPlaceholder.
- Bootstrap and client-side validation scripts are loaded from CDNs.
- Response caching is configured for the details response. Avoid adding patient-specific content to that cached response in a real production app.
