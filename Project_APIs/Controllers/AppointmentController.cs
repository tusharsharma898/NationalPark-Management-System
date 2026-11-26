
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Project_APIs.Models;
using Project_APIs.Models.DTOs;
using Project_APIs.Repository.IRepository;

namespace Project_APIs.Controllers
{
    [Route("api/appointment")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly INationalParkRepository _nationalParkRepository;
        private readonly IMapper _mapper;

        // Temporary testing limit. Change to 100 when finished.
        private const int DailyCapacity = 10;

        public AppointmentsController(
            IAppointmentRepository appointmentRepository,
            INationalParkRepository nationalParkRepository,
            IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _nationalParkRepository = nationalParkRepository;
            _mapper = mapper;
        }

        // GET: api/appointment
        [HttpGet]
        public IActionResult GetAppointments()
        {
            var appointments =
                _appointmentRepository.GetAppointments();

            var result =
                _mapper.Map<List<AppointmentDto>>(appointments);

            return Ok(result);
        }

        // GET: api/appointment/1
        [HttpGet("{id:int}", Name = "GetAppointment")]
        public IActionResult GetAppointment(int id)
        {
            var appointment =
                _appointmentRepository.GetAppointment(id);

            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            var result =
                _mapper.Map<AppointmentDto>(appointment);

            return Ok(result);
        }

        // POST: api/appointment
        [HttpPost]
        public IActionResult CreateAppointment(
            [FromBody] AppointmentDto appointmentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int totalPeople =
                appointmentDto.AdultCount +
                appointmentDto.ChildCount;

            if (totalPeople < 1 || totalPeople > 5)
            {
                return BadRequest(
                    "You can book between 1 and 5 tickets per appointment.");
            }

            var nationalPark =
                _nationalParkRepository.GetNationalPark(
                    appointmentDto.NationalParkId);

            if (nationalPark == null)
            {
                return BadRequest("National park not found.");
            }

            // Count visitors for this park on this visit date.
            int alreadyBooked =
                _appointmentRepository.GetBookedVisitors(
                    appointmentDto.VisitDate,
                    appointmentDto.NationalParkId);

            if (alreadyBooked + totalPeople > DailyCapacity)
            {
                int availableSpots =
                    Math.Max(0, DailyCapacity - alreadyBooked);

                return BadRequest(
                    $"Only {availableSpots} visitor spots are available " +
                    "for this National Park on the selected date.");
            }

            var appointment =
                _mapper.Map<Appointment>(appointmentDto);

            appointment.NationalParkId = nationalPark.Id;
            appointment.NationalPark = nationalPark;

            // Example ticket prices; replace with your actual prices.
            const decimal adultPrice = 200m;
            const decimal childPrice = 100m;

            appointment.AdultPrice =
                appointment.AdultCount * adultPrice;

            appointment.ChildPrice =
                appointment.ChildCount * childPrice;

            appointment.TotalPrice =
                appointment.AdultPrice + appointment.ChildPrice;

            // Server controls the booking status.
            appointment.Status = "Pending";

            if (!_appointmentRepository.CreateAppointment(appointment))
            {
                return StatusCode(
                    500,
                    "An error occurred while creating the appointment.");
            }

            var result =
                _mapper.Map<AppointmentDto>(appointment);

            return CreatedAtRoute(
                "GetAppointment",
                new { id = appointment.Id },
                result);

        }
        [HttpPut("{id:int}/payment")]
        public IActionResult UpdatePayment(int id, [FromBody] string transactionId)
        {
            var appointment = _appointmentRepository.GetAppointment(id);

            if (appointment == null)
                return NotFound();

            appointment.Status = "Paid";
            appointment.TransactionId = transactionId;

            if (!_appointmentRepository.UpdateAppointment(appointment))
                return BadRequest();

            return NoContent();
        }
        [HttpPut("{id:int}/cancel")]
        public IActionResult CancelAppointment(int id)
        {
            var appointment = _appointmentRepository.GetAppointment(id);

            if (appointment == null)
                return NotFound("Appointment not found.");

            if (appointment.Status == "Cancelled")
                return BadRequest("Appointment is already cancelled.");

            appointment.Status = "Cancelled";

            if (!_appointmentRepository.UpdateAppointment(appointment))
                return BadRequest("Failed to cancel appointment.");

            return Ok(appointment);
        }

    }
}
