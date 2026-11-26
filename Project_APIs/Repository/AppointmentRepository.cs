using Microsoft.EntityFrameworkCore;
using Project_APIs.Data;
using Project_APIs.Models;
using Project_APIs.Repository.IRepository;
using System.Linq;

namespace Project_APIs.Repository
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool CreateAppointment(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            return Save();
        }

        public bool DeleteAppointment(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
            return Save();
        }

        public Appointment GetAppointment(int appointmentId)
        {
            return _context.Appointments
                .Include(a => a.NationalPark)
                .FirstOrDefault(a => a.Id == appointmentId);
        }

        public ICollection<Appointment> GetAppointments()
        {
            return _context.Appointments
                .Include(a => a.NationalPark)
                .ToList();
        }

        public bool AppointmentExists(int appointmentId)
        {
            return _context.Appointments
                .Any(a => a.Id == appointmentId);
        }

        public bool UpdateAppointment(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            return Save();
        }

        public int GetBookedVisitors(DateTime visitDate, int nationalParkId)
        {
            DateTime startDate = visitDate.Date;
            DateTime endDate = startDate.AddDays(1);

            return _context.Appointments
                .Where(a =>
                    a.VisitDate >= startDate &&
                    a.VisitDate < endDate &&
                    a.NationalParkId == nationalParkId &&
                    (a.Status == "Pending" ||
                     a.Status == "Confirmed" ||
                     a.Status == "Paid"))
                .Sum(a => (int?)(a.AdultCount + a.ChildCount)) ?? 0;
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}