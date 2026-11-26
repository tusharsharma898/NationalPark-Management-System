using Project_APIs.Models;

namespace Project_APIs.Repository.IRepository
{
    public interface IAppointmentRepository
    {
        ICollection<Appointment> GetAppointments();
        Appointment GetAppointment(int appointmentId);
        bool AppointmentExists(int appointmentId);
        bool CreateAppointment(Appointment appointment);
        bool UpdateAppointment(Appointment appointment);
        bool DeleteAppointment(Appointment appointment);
        int GetBookedVisitors(DateTime visitDate, int nationalParkId);
        bool Save();
    }
}
