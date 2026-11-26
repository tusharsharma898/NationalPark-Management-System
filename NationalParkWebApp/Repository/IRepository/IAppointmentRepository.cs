using NationalParkWebApp.Models;

namespace NationalParkWebApp.Repository.IRepository
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<Appointment> CreateAppointmentAsync(string url, Appointment appointment);
    }
}
