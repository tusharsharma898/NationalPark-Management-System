using NationalParkWebApp.Models;

namespace NationalParkWebApp.Models.ViewModels
{
    public class AppointmentVM
    {
        public Appointment Appointment { get; set; }
        public NationalPark NationalPark { get; set; }
        public IEnumerable<Trail> TrailList { get; set; }
    }
}